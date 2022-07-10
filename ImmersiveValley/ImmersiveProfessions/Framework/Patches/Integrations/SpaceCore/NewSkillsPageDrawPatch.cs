namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using DaLion.Common;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Prestige;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Textures;

#endregion using directives

[UsedImplicitly]
internal sealed class NewSkillsPageDrawPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal NewSkillsPageDrawPatch()
    {
        try
        {
            Target = "SpaceCore.Interface.NewSkillsPage".ToType().RequireMethod("draw", new[] { typeof(SpriteBatch) });
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to overlay skill bars above skill level 10 + draw prestige ribbons on the skills page.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? NewSkillsPageDrawTranspiler(
        IEnumerable<CodeInstruction> instructions,
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
            Log.E($"Failed adjusing localized skill page content position.\nHelper returned {ex}");
            return null;
        }

        /// Injected: DrawExtendedLevelBars(levelIndex, skillindex, x, y, addedX, skillLevel, b)
        /// Before: if (i == 9) draw level number ...

        // local variable indices correspond to SpaceCore v1.8.0
        var levelIndex = helper.Locals[8];
        var skillIndex = helper.Locals[9];
        var skillLevel = helper.Locals[13];
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_S, levelIndex),
                    new CodeInstruction(OpCodes.Ldc_I4_S, 9),
                    new CodeInstruction(OpCodes.Bne_Un)
                )
                .StripLabels(out var labels)
                .InsertWithLabels(
                    labels,
                    new CodeInstruction(OpCodes.Ldloc_S, levelIndex),
                    new CodeInstruction(OpCodes.Ldloc_S, skillIndex),
                    new CodeInstruction(OpCodes.Ldloc_0), // load x
                    new CodeInstruction(OpCodes.Ldloc_1), // load y
                    new CodeInstruction(OpCodes.Ldloc_3), // load xOffset
                    new CodeInstruction(OpCodes.Ldloc_S, skillLevel),
                    new CodeInstruction(OpCodes.Ldarg_1), // load b
                    new CodeInstruction(OpCodes.Call,
                        typeof(SkillsPageDrawPatch).RequireMethod(nameof(SkillsPageDrawPatch.DrawExtendedLevelBars)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw SpaceCore skills page extended level bars.\nHelper returned {ex}");
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
            Log.E($"Failed while patching to draw max skill level with different color.\nHelper returned {ex}");
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
                        typeof(SkillsPageDrawPatch).RequireMethod(nameof(SkillsPageDrawPatch.DrawRibbons)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching to draw skills page prestige ribbons.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}