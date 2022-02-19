namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;
using Prestige;

#endregion using directives

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
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (8)")
                )
                .GetOperand(out var levelIndex)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (9)")
                )
                .GetOperand(out var skillIndex)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(int)} (13)")
                )
                .GetOperand(out var skillLevel)
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, levelIndex),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                    new CodeInstruction(OpCodes.Bne_Un)
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
                        typeof(SkillsPageDrawPatch).MethodNamed(nameof(SkillsPageDrawPatch.DrawExtendedLevelBars)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw SpaceCore skills page extended level bars. Helper returned {ex}");
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
                    new CodeInstruction(OpCodes.Ldfld, typeof(SkillsPage).Field("hoverText")),
                    new CodeInstruction(OpCodes.Callvirt, typeof(string).PropertyGetter(nameof(string.Length)))
                )
                .StripLabels(out var labels) // backup and remove branch labels
                .Insert(
                    labels,
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call,
                        typeof(SkillsPageDrawPatch).MethodNamed(nameof(SkillsPageDrawPatch.DrawRibbons)))
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
}