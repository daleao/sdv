namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Textures;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPageDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillsPageDrawPatch()
    {
        Target = RequireMethod<SkillsPage>(nameof(SkillsPage.draw), new[] { typeof(SpriteBatch) });
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? SkillsPageDrawTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Inject: x -= ModEntry.Config.PrestigeProgressionStyle == ProgressionStyle.Stars ? Textures.STARS_WIDTH_I : Textures.RIBBON_WIDTH_I;
        /// After: x = ...

        var notRibbons = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Call,
                        typeof(LocalizedContentManager).RequirePropertyGetter(nameof(LocalizedContentManager
                            .CurrentLanguageCode)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Br_S)
                )
                .GetOperand(out var resumeExecution)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Stloc_0)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.EnablePrestige))),
                    new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                    new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                    new CodeInstruction(OpCodes.Call,
                        typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.PrestigeProgressionStyle))),
                    new CodeInstruction(OpCodes.Ldc_I4_0),
                    new CodeInstruction(OpCodes.Beq_S, notRibbons),
                    new CodeInstruction(OpCodes.Ldc_I4_S,
                        (int)((Textures.RIBBON_WIDTH_I + 5) * Textures.RIBBON_SCALE_F)),
                    new CodeInstruction(OpCodes.Sub),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                )
                .InsertWithLabels(
                    new[] { notRibbons },
                    new CodeInstruction(OpCodes.Ldc_I4_S,
                        (int)((Textures.STARS_WIDTH_I + 4) * Textures.STARS_SCALE_F)),
                    new CodeInstruction(OpCodes.Sub)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed adjusing localized skill page content position. Helper returned {ex}");
            return null;
        }

        /// Injected: DrawExtendedLevelBars(levelIndex, skillIndex, x, y, addedX, skillLevel, b)
        /// Before: if (i == 9) draw level number ...

        var skillIndex = helper.Locals[4];
        var skillLevel = helper.Locals[8];
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                    new CodeInstruction(OpCodes.Bne_Un)
                )
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_3), // load levelIndex
                    new CodeInstruction(OpCodes.Ldloc_S, skillIndex),
                    new CodeInstruction(OpCodes.Ldloc_0), // load x
                    new CodeInstruction(OpCodes.Ldloc_1), // load y
                    new CodeInstruction(OpCodes.Ldloc_2), // load addedX
                    new CodeInstruction(OpCodes.Ldloc_S, skillLevel), // load skillLevel,
                    new CodeInstruction(OpCodes.Ldarg_1), // load b
                    new CodeInstruction(OpCodes.Call,
                        typeof(SkillsPageDrawPatch).RequireMethod(nameof(DrawExtendedLevelBars)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw skills page extended level bars. Helper returned {ex}");
            return null;
        }

        /// From: (addedSkill ? Color.LightGreen : Color.Cornsilk)
        /// To: (addedSkill ? Color.LightGreen : skillLevel == 20 ? Color.Grey : Color.SandyBrown)

        var isSkillLevel20 = generator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.SandyBrown)))
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 20),
                    new CodeInstruction(OpCodes.Beq_S, isSkillLevel20)
                )
                .Advance()
                .GetOperand(out var resumeExecution)
                .Advance()
                .InsertWithLabels(
                    new[] { isSkillLevel20 },
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Cornsilk))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw max skill level with different color. Helper returned {ex}");
            return null;
        }

        /// Injected: DrawRibbonsSubroutine(b);
        /// Before: if (hoverText.Length > 0)

        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(SkillsPage).RequireField("hoverText")),
                    new CodeInstruction(OpCodes.Callvirt, typeof(string).RequirePropertyGetter(nameof(string.Length)))
                )
                .StripLabels(out var labels) // backup and remove branch labels
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SkillsPageDrawPatch).RequireMethod(nameof(DrawRibbons)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw skills page prestige ribbons. Helper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    internal static void DrawExtendedLevelBars(int levelIndex, int skillIndex, int x, int y, int addedX,
        int skillLevel, SpriteBatch b)
    {
        if (!ModEntry.Config.EnablePrestige) return;

        var drawBlue = skillLevel > levelIndex + 10;
        if (!drawBlue) return;

        // this will draw only the blue bars
        if ((levelIndex + 1) % 5 != 0)
            b.Draw(Textures.BarsTx, new(addedX + x + levelIndex * 36, y - 4 + skillIndex * 56),
                new(0, 0, 8, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }

    internal static void DrawRibbons(SkillsPage page, SpriteBatch b)
    {
        if (!ModEntry.Config.EnablePrestige) return;

        var position =
            new Vector2(
                page.xPositionOnScreen + page.width + Textures.PROGRESSION_HORIZONTAL_OFFSET_I,
                page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth +
                Textures.PROGRESSION_VERTICAL_OFFSET_I);
        if (ModEntry.Config.PrestigeProgressionStyle == ModConfig.ProgressionStyle.StackedStars)
        {
            position.X -= 22;
            position.Y -= 4;
        }

        for (var i = 0; i < 5; ++i)
        {
            position.Y += 56;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i)
            };
            var count = Game1.player.GetProfessionsForSkill(skill, true).Count();
            if (count == 0) continue;

            Rectangle srcRect;
            if (ModEntry.Config.PrestigeProgressionStyle.ToString().Contains("Ribbons"))
            {
                srcRect = new(i * Textures.RIBBON_WIDTH_I, (count - 1) * Textures.RIBBON_WIDTH_I,
                    Textures.RIBBON_WIDTH_I, Textures.RIBBON_WIDTH_I);
            }
            else if (ModEntry.Config.PrestigeProgressionStyle == ModConfig.ProgressionStyle.StackedStars)
            {
                srcRect = new(0, (count - 1) * 16, Textures.STARS_WIDTH_I, 16);
            }
            else
            {
                srcRect = Rectangle.Empty;
            }

            b.Draw(Textures.ProgressionTx, position, srcRect, Color.White, 0f, Vector2.Zero,
                ModEntry.Config.PrestigeProgressionStyle == ModConfig.ProgressionStyle.StackedStars
                    ? Textures.STARS_SCALE_F
                    : Textures.RIBBON_SCALE_F, SpriteEffects.None, 1f);
        }

        if (ModEntry.SpaceCoreApi is null) return;

        if (ModEntry.Config.PrestigeProgressionStyle.ToString().Contains("Ribbons"))
            position.X += 2; // not sure why but custom skill ribbons render with a small offset

        foreach (var skill in ModEntry.CustomSkills.Values)
        {
            position.Y += 56;
            var count = Game1.player.GetProfessionsForSkill(skill, true).Count();
            if (count == 0) continue;

            var srcRect = ModEntry.Config.PrestigeProgressionStyle switch
            {
                ModConfig.ProgressionStyle.Gen3Ribbons or ModConfig.ProgressionStyle.Gen4Ribbons => new(
                    skill.StringId == "blueberry.LoveOfCooking.CookingSkill" ? 111 : 133,
                    (count - 1) * Textures.RIBBON_WIDTH_I, Textures.RIBBON_WIDTH_I, Textures.RIBBON_WIDTH_I),
                ModConfig.ProgressionStyle.StackedStars =>
                    new(0, (count - 1) * 16, Textures.STARS_WIDTH_I, 16),
                _ => Rectangle.Empty
            };

            b.Draw(Textures.ProgressionTx, position, srcRect, Color.White, 0f, Vector2.Zero,
                ModEntry.Config.PrestigeProgressionStyle == ModConfig.ProgressionStyle.StackedStars
                    ? Textures.STARS_SCALE_F
                    : Textures.RIBBON_SCALE_F, SpriteEffects.None, 1f);
        }
    }

    #endregion injected subroutines
}