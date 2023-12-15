namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPageDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsPageDrawPatcher"/> class.</summary>
    internal SkillsPageDrawPatcher()
    {
        this.Target = this.RequireMethod<SkillsPage>(nameof(SkillsPage.draw), new[] { typeof(SpriteBatch) });
        this.Transpiler!.before = new[] { "Shockah.XPDisplay" };
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    [HarmonyBefore("Shockah.XPDisplay")]
    private static IEnumerable<CodeInstruction>? SkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        // Inject: x -= ProfessionsModule.Config.PrestigeRibbonStyle == RibbonStyle.Stars ? Textures.STARS_WIDTH_I : Textures.RIBBON_WIDTH_I;
        // After: x = ...
        try
        {
            var notRibbons = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(LocalizedContentManager).RequirePropertyGetter(nameof(LocalizedContentManager
                                .CurrentLanguageCode))),
                    })
                .Match(new[] { new CodeInstruction(OpCodes.Br_S) })
                .GetOperand(out var resumeExecution)
                .Match(new[] { new CodeInstruction(OpCodes.Stloc_0) })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ProfessionsModule).RequirePropertyGetter(nameof(ProfessionsModule.EnableSkillReset))),
                        new CodeInstruction(OpCodes.Brfalse_S, resumeExecution),
                        new CodeInstruction(OpCodes.Call, typeof(ModEntry).RequirePropertyGetter(nameof(ModEntry.Config))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ModConfig).RequirePropertyGetter(nameof(ModConfig.Professions))),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(ProfessionConfig).RequirePropertyGetter(
                                nameof(ProfessionConfig.PrestigeRibbonStyle))),
                        new CodeInstruction(OpCodes.Ldc_I4_0),
                        new CodeInstruction(OpCodes.Beq_S, notRibbons),
                        new CodeInstruction(
                            OpCodes.Ldc_I4_S,
                            (int)((Textures.RibbonWidth + 5) * Textures.RibbonScale)),
                        new CodeInstruction(OpCodes.Sub),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Ldc_I4_S,
                            (int)((Textures.StarsWidth + 4) * Textures.StarsScale)),
                        new CodeInstruction(OpCodes.Sub),
                    },
                    new[] { notRibbons });
        }
        catch (Exception ex)
        {
            Log.E($"Failed adjusting localized skill page content position.\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawExtendedLevelBars(levelIndex, skillIndex, x, y, addedX, skillLevel, b)
        // Before: if (i == 9) draw level number ...
        var skillIndex = helper.Locals[4];
        var skillLevel = helper.Locals[8];
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_3),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                        new CodeInstruction(OpCodes.Bne_Un),
                    },
                    ILHelper.SearchOption.First)
                .StripLabels(out var labels)
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_3), // load levelIndex
                        new CodeInstruction(OpCodes.Ldloc_S, skillIndex),
                        new CodeInstruction(OpCodes.Ldloc_0), // load x
                        new CodeInstruction(OpCodes.Ldloc_1), // load y
                        new CodeInstruction(OpCodes.Ldloc_2), // load addedX
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel), // load skillLevel,
                        new CodeInstruction(OpCodes.Ldarg_1), // load b
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillsPageDrawPatcher).RequireMethod(nameof(DrawExtendedLevelBars))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching to draw skills page extended level bars.\nHelper returned {ex}");
            return null;
        }

        // From: (addedSkill ? Color.LightGreen : Color.Cornsilk)
        // To: (addedSkill ? Color.LightGreen : skillLevel == 20 ? Color.Grey : Color.SandyBrown)
        try
        {
            var isSkillLevel20 = generator.DefineLabel();
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Color).RequirePropertyGetter(nameof(Color.SandyBrown))),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                        new CodeInstruction(OpCodes.Ldc_I4_S, 20),
                        new CodeInstruction(OpCodes.Beq_S, isSkillLevel20),
                    })
                .Move()
                .GetOperand(out var resumeExecution)
                .Move()
                .Insert(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(Color).RequirePropertyGetter(nameof(Color.Cornsilk))),
                        new CodeInstruction(OpCodes.Br_S, resumeExecution),
                    },
                    new[] { isSkillLevel20 });
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching to draw max skill level with different color.\nHelper returned {ex}");
            return null;
        }

        // Injected: DrawRibbonsSubroutine(b);
        // Before: if (hoverText.Length > 0)
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldfld, typeof(SkillsPage).RequireField("hoverText")),
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(string).RequirePropertyGetter(nameof(string.Length))),
                    },
                    ILHelper.SearchOption.Last)
                .StripLabels(out var labels) // backup and remove branch labels
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(SkillsPageDrawPatcher).RequireMethod(nameof(DrawRibbons))),
                    },
                    labels);
        }
        catch (Exception ex)
        {
            Log.E($"Failed patching to draw skills page prestige ribbons.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "Harmony-injected subroutine shared by a SpaceCore patch.")]
    internal static void DrawExtendedLevelBars(
        int levelIndex, int skillIndex, int x, int y, int addedX, int skillLevel, SpriteBatch b)
    {
        if (!ProfessionsModule.EnablePrestigeLevels)
        {
            return;
        }

        var drawBlue = skillLevel > levelIndex + 10;
        if (!drawBlue)
        {
            return;
        }

        // this will draw only the blue bars
        if ((levelIndex + 1) % 5 != 0)
        {
            b.Draw(
                Textures.SkillBarsTx,
                new Vector2(addedX + x + (levelIndex * 36), y - 4 + (skillIndex * 56)),
                new Rectangle(0, 0, 8, 9),
                Color.White,
                0f,
                Vector2.Zero,
                4f,
                SpriteEffects.None,
                1f);
        }
    }

    internal static void DrawRibbons(SkillsPage page, SpriteBatch b)
    {
        if (!ProfessionsModule.EnableSkillReset)
        {
            return;
        }

        var position =
            new Vector2(
                page.xPositionOnScreen + page.width + Textures.ProgressionHorizontalOffset,
                page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Textures.ProgressionVerticalOffset);
        if (ProfessionsModule.Config.PrestigeRibbonStyle == ProfessionConfig.RibbonStyle.StackedStars)
        {
            position.X -= 22;
            position.Y -= 4;
        }

        for (var i = 0; i < 5; i++)
        {
            position.Y += 56;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i),
            };
            var count = Game1.player.GetProfessionsForSkill(skill, true).Length;
            if (count == 0)
            {
                continue;
            }

            Rectangle sourceRect;
            if (ProfessionsModule.Config.PrestigeRibbonStyle.ToString().Contains("Ribbons"))
            {
                sourceRect = new Rectangle(
                    i * Textures.RibbonWidth,
                    (count - 1) * Textures.RibbonWidth,
                    Textures.RibbonWidth,
                    Textures.RibbonWidth);
            }
            else if (ProfessionsModule.Config.PrestigeRibbonStyle == ProfessionConfig.RibbonStyle.StackedStars)
            {
                sourceRect = new Rectangle(0, (count - 1) * 16, Textures.StarsWidth, 16);
            }
            else
            {
                sourceRect = Rectangle.Empty;
            }

            b.Draw(
                Textures.PrestigeSheetTx,
                position,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                ProfessionsModule.Config.PrestigeRibbonStyle == ProfessionConfig.RibbonStyle.StackedStars
                    ? Textures.StarsScale
                    : Textures.RibbonScale,
                SpriteEffects.None,
                1f);
        }

        if (CustomSkill.Loaded.Count == 0)
        {
            return;
        }

        if (ProfessionsModule.Config.PrestigeRibbonStyle.ToString().Contains("Ribbons"))
        {
            position.X += 2; // not sure why but custom skill ribbons render with a small offset
        }

        var customSkills = SpaceCoreIntegration.Instance!.ModApi!
            .GetCustomSkills()
            .Select(name => CustomSkill.Loaded[name]);
        if (LuckSkill.Instance is not null)
        {
            // luck skill must be enumerated first
            customSkills = customSkills.Prepend(LuckSkill.Instance);
        }

        foreach (var skill in customSkills)
        {
            position.Y += 56;

            var count = Game1.player.GetProfessionsForSkill(skill, true).Length;
            if (count == 0)
            {
                continue;
            }

            var sourceRect = ProfessionsModule.Config.PrestigeRibbonStyle switch
            {
                ProfessionConfig.RibbonStyle.Gen3Ribbons or ProfessionConfig.RibbonStyle.Gen4Ribbons => new Rectangle(
                    skill.StringId == "blueberry.LoveOfCooking.CookingSkill" ? 111 : 133,
                    (count - 1) * Textures.RibbonWidth,
                    Textures.RibbonWidth,
                    Textures.RibbonWidth),
                ProfessionConfig.RibbonStyle.StackedStars =>
                    new Rectangle(0, (count - 1) * 16, Textures.StarsWidth, 16),
                _ => Rectangle.Empty,
            };

            b.Draw(
                Textures.PrestigeSheetTx,
                position,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                ProfessionsModule.Config.PrestigeRibbonStyle == ProfessionConfig.RibbonStyle.StackedStars
                    ? Textures.StarsScale
                    : Textures.RibbonScale,
                SpriteEffects.None,
                1f);
        }
    }

    #endregion injected subroutines
}
