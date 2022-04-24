namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class BushMachineGetOutputPatch : BasePatch
{
    private static MethodInfo _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal BushMachineGetOutputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType().RequireMethod("GetOutput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for automated Berry Bush forage increment.</summary>
    [HarmonyPostfix]
    private static void BushMachineGetOutputPostfix(object __instance)
    {
        if (__instance is null || !ModEntry.Config.ShouldCountAutomatedHarvests) return;

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
        var machine = (Bush) _GetMachine.Invoke(__instance, null);
        if (machine is null || machine.size.Value >= Bush.greenTeaBush) return;

        if (!Context.IsMainPlayer || !Game1.player.HasProfession(Profession.Ecologist)) return;

        Game1.player.IncrementData<uint>(DataField.EcologistItemsForaged);
    }

    /// <summary>Patch for automated Berry Bush quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> BushMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: int quality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : 0);
        /// To: int quality = Game1.player.professions.Contains(<ecologist_id>) ? GetEcologistForageQuality : 0);

        try
        {
            helper
                .FindProfessionCheck((int) Profession.Ecologist) // find index of ecologist check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4) // quality = 4
                )
                .GetLabels(out var labels) // backup branch labels
                .ReplaceWith( // replace with custom quality
                    new(OpCodes.Call,
                        typeof(FarmerExtensions).RequireMethod(nameof(FarmerExtensions.GetEcologistForageQuality)))
                )
                .InsertWithLabels(
                    labels: labels, // restore backed-up labels
                    new CodeInstruction(OpCodes.Call, typeof(Game1).RequirePropertyGetter(nameof(Game1.player)))
                );
        }
        catch (Exception ex)
        {
            Log.E($"Failed while patching automated Berry Bush quality.\nHelper returned {ex}");
            transpilationFailed = true;
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}