namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;
    
#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;
using Utility;

#endregion using directives

[UsedImplicitly]
internal class SkillsPageDrawPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillsPageDrawPatch()
    {
        Original = RequireMethod<SkillsPage>(nameof(SkillsPage.draw), new[] {typeof(SpriteBatch)});
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SkillsPageDrawTranspiler(IEnumerable<CodeInstruction> instructions,
        ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

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
            transpilationFailed = true;
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
                    new[] {isSkillLevel20},
                    new CodeInstruction(OpCodes.Call, typeof(Color).RequirePropertyGetter(nameof(Color.Cornsilk))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw max skill level with different color. Helper returned {ex}");
            transpilationFailed = true;
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
            transpilationFailed = true;
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
            b.Draw(Textures.SkillBarTx, new(addedX + x + levelIndex * 36, y - 4 + skillIndex * 56),
                new(0, 0, 8, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }

    internal static void DrawRibbons(SkillsPage page, SpriteBatch b)
    {
        if (!ModEntry.Config.EnablePrestige) return;

        var position =
            new Vector2(
                page.xPositionOnScreen + page.width + Textures.RIBBON_HORIZONTAL_OFFSET_I,
                page.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - 70);
        for (var i = 0; i < 5; ++i)
        {
            position.Y += 56;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skillIndex = i switch
            {
                1 => 3,
                3 => 1,
                _ => i
            };

            var count = Game1.player.NumberOfProfessionsInSkill(skillIndex, true);
            if (count == 0) continue;

            var srcRect = new Rectangle(i * Textures.RIBBON_WIDTH_I, (count - 1) * Textures.RIBBON_WIDTH_I,
                Textures.RIBBON_WIDTH_I, Textures.RIBBON_WIDTH_I);
            b.Draw(Textures.RibbonTx, position, srcRect, Color.White, 0f, Vector2.Zero, Textures.RIBBON_SCALE_F,
                SpriteEffects.None, 1f);
        }
    }

    #endregion injected subroutines
}