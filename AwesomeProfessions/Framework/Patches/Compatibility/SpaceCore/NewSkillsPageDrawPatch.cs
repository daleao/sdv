using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using TheLion.Stardew.Professions.Framework.Patches.Prestige;
using TheLion.Stardew.Professions.Framework.Utility;

namespace TheLion.Stardew.Professions.Framework.Patches.Compatibility;

[UsedImplicitly]
internal class NewSkillsPageDrawPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal NewSkillsPageDrawPatch()
    {
        try
        {
            Original = "SpaceCore.Interface.NewSkillsPage".ToType()
                .MethodNamed("draw", new[] {typeof(SpriteBatch)});
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> NewSkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions,
        ILGenerator iLGenerator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: DrawExtendedLevelBars(i, j, x, y, addedX, skillLevel, b)
        /// Before: if (i == 9) draw level number ...

        // local variable indices correspond to SpaceCore v1.8.0
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (10)")
                )
                .GetOperand(out var levelIndex)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (11)")
                )
                .GetOperand(out var skillIndex)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (15)")
                )
                .GetOperand(out var skillLevel)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, levelIndex),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                    new CodeInstruction(OpCodes.Ceq)
                )
                .StripLabels(out var labels)
                .Insert(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_S, levelIndex),
                    new CodeInstruction(OpCodes.Ldloc_S, skillIndex),
                    new CodeInstruction(OpCodes.Ldloc_0), // load x
                    new CodeInstruction(OpCodes.Ldloc_1), // load y
                    new CodeInstruction(OpCodes.Ldloc_3), // load xOffset
                    new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                    new CodeInstruction(OpCodes.Ldarg_1), // load b
                    new CodeInstruction(OpCodes.Call,
                        typeof(SkillsPageDrawPatch).MethodNamed(nameof(DrawExtendedLevelBars)))
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log(
                $"Failed while patching to draw SpaceCore skills page extended level bars. Helper returned {ex}",
                LogLevel.Error);
            return null;
        }

        /// From: (addedSkill ? Color.LightGreen : Color.Cornsilk)
        /// To: (addedSkill ? Color.LightGreen : skillLevel == 20 ? Color.Grey : Color.SandyBrown)

        var isSkillLevel20 = iLGenerator.DefineLabel();
        try
        {
            helper
                .FindNext(
                    new CodeInstruction(OpCodes.Call, typeof(Color).PropertyGetter(nameof(Color.SandyBrown)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldloc_S)
                )
                .GetOperand(out var skillLevel)
                .Return()
                .Insert(
                    new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 20),
                    new CodeInstruction(OpCodes.Beq_S, isSkillLevel20)
                )
                .Advance()
                .GetOperand(out var resumeExecution)
                .Advance()
                .Insert(
                    new[] {isSkillLevel20},
                    new CodeInstruction(OpCodes.Call, typeof(Color).PropertyGetter(nameof(Color.Cornsilk))),
                    new CodeInstruction(OpCodes.Br_S, resumeExecution)
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching to draw max skill level with different color. Helper returned {ex}",
                LogLevel.Error);
            return null;
        }

        /// Injected: DrawRibbonsSubroutine(b);
        /// Before: if (hoverText.Length > 0)

        try
        {
            helper
                .FindLast(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, typeof(SkillsPage).Field("hoverText")),
                    new CodeInstruction(OpCodes.Callvirt, typeof(string).PropertyGetter(nameof(string.Length)))
                )
                .StripLabels(out var labels) // backup and remove branch labels
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SkillsPageDrawPatch).MethodNamed(nameof(DrawRibbons)))
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching to draw skills page prestige ribbons. Helper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region private methods

    private static void DrawExtendedLevelBars(int levelIndex, int skillIndex, int x, int y, int addedX,
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

    private static void DrawRibbons(IClickableMenu page, SpriteBatch b)
    {
        if (!ModEntry.Config.EnablePrestige) return;

        var w = Textures.RibbonWidth;
        var s = Textures.RibbonScale;
        var position =
            new Vector2(
                page.xPositionOnScreen + page.width + Textures.RibbonHorizontalOffset,
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

            var srcRect = new Rectangle(i * w, (count - 1) * w, w, w);
            b.Draw(Textures.RibbonTx, position, srcRect, Color.White, 0f, Vector2.Zero, s,
                SpriteEffects.None, 1f);
        }
    }

    #endregion private methods
}