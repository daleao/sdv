namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.AnimalHusbandryMod;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Extensions;
using Stardew.Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal class FeedingBasketOverridesDoFunctionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FeedingBasketOverridesDoFunctionPatch()
    {
        try
        {
            Original = "AnimalHusbandryMod.tools.FeedingBasketOverrides".ToType()
                .MethodNamed("DoFunction");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for Rancher to combine Shepherd and Coopmaster friendship bonus.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> InseminationSyringeOverridesDoFunctionTranspiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator generator, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: if ((!animal.isCoopDweller() && who.professions.Contains(3)) || (animal.isCoopDweller() && who.professions.Contains(2)))
        /// To: if (who.professions.Contains(<rancher_id>)
        /// -- and also
        /// Injected: if (who.professions.Contains(<rancher_id> + 100)) repeat professionAdjust ...

        var isNotPrestiged = generator.DefineLabel();
        try
        {
            helper
                .FindFirst(
                    new CodeInstruction(OpCodes.Ldloc_1),
                    new CodeInstruction(OpCodes.Callvirt,
                        typeof(FarmAnimal).MethodNamed(nameof(FarmAnimal.isCoopDweller)))
                )
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldloc_S, $"{typeof(double)} (7)"),
                    new CodeInstruction(OpCodes.Ldsfld),
                    new CodeInstruction(OpCodes.Ldfld)
                )
                .RetreatUntil(
                    new CodeInstruction(OpCodes.Brfalse_S)
                )
                .GetOperand(out var isNotRancher)
                .Return(2)
                .RemoveUntil(
                    new CodeInstruction(OpCodes.Nop)
                )
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 5)
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Rancher, (Label) isNotRancher)
                .ToBufferUntil(
                    new CodeInstruction(OpCodes.Stloc_S, $"{typeof(double)} (7)")
                )
                .InsertBuffer()
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_S, (byte) 5)
                )
                .InsertProfessionCheckForPlayerOnStack((int) Profession.Rancher + 100, isNotPrestiged)
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Nop)
                )
                .Remove()
                .AddLabels(isNotPrestiged);

        }
        catch (Exception ex)
        {
            Log.E(
                $"Failed while moving combined feeding basket Coopmaster + Shepherd friendship bonuses to Rancher.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}