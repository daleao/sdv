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

namespace TheLion.Stardew.Professions.Framework.Patches;

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
        MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// Injected: DrawExtendedLevelBars(i, j, x, y, addedX, skillLevel, b)
        /// Before: if (i == 9) draw level number ...

        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (4)")
                )
                .GetOperand(out var j)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (8)")
                )
                .GetOperand(out var skillLevel)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                    new CodeInstruction(OpCodes.Bne_Un)
                )
                .StripLabels(out var labels)
                .Insert(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_3), // load i (profession index)
                    new CodeInstruction(OpCodes.Ldloc_S, j), // load j (skill index)
                    new CodeInstruction(OpCodes.Ldloc_0), // load x
                    new CodeInstruction(OpCodes.Ldloc_1), // load y
                    new CodeInstruction(OpCodes.Ldloc_2), // load addedX
                    new CodeInstruction(OpCodes.Ldloc_S, skillLevel), // load skillLevel,
                    new CodeInstruction(OpCodes.Ldarg_1), // load b
                    new CodeInstruction(OpCodes.Call,
                        typeof(SkillsPageDrawPatch).MethodNamed(nameof(DrawExtendedLevelBars)))
                );
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching to draw skills page extended level bars. Helper returned {ex}",
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
                        typeof(SkillsPageDrawPatch).MethodNamed(nameof(DrawRibbonsSubroutine)))
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
            b.Draw(Utility.Prestige.SkillBarTx, new(addedX + x + levelIndex * 36, y - 4 + skillIndex * 56),
                new(0, 0, 8, 9), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
    }

    private static void DrawRibbonsSubroutine(SkillsPage page, SpriteBatch b)
    {
        if (!ModEntry.Config.EnablePrestige) return;

        var w = Utility.Prestige.RibbonWidth;
        var s = Utility.Prestige.RibbonScale;
        var position =
            new Vector2(
                page.xPositionOnScreen + page.width + Utility.Prestige.RibbonHorizontalOffset,
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
            b.Draw(Utility.Prestige.RibbonTx, position, srcRect, Color.White, 0f, Vector2.Zero, s,
                SpriteEffects.None, 1f);
        }
    }

    #endregion private methods
}