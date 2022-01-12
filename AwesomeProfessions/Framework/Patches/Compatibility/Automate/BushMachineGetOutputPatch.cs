using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using Object = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches.Compatibility;

[UsedImplicitly]
internal class BushMachineGetOutputPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal BushMachineGetOutputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType()
                .MethodNamed("GetOutput");
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

        var machine = ModEntry.ModHelper.Reflection.GetProperty<Object>(__instance, "Machine").GetValue();
        if (machine is null) return;

        var who = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!who.HasProfession("Ecologist")) return;

        ModData.Increment<uint>(DataField.EcologistItemsForaged, who);
    }

    /// <summary>Patch for automated Berry Bush quality.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> BushMachineGetOutputTranspiler(
        IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        var helper = new ILHelper(original, instructions);

        /// From: int quality = Game1.player.professions.Contains(<ecologist_id>) ? 4 : 0);
        /// To: int quality = Game1.player.professions.Contains(<ecologist_id>) ? GetEcologist : 0);

        try
        {
            helper
                .FindProfessionCheck(Utility.Professions.IndexOf("Ecologist")) // find index of ecologist check
                .AdvanceUntil(
                    new CodeInstruction(OpCodes.Ldc_I4_4) // quality = 4
                )
                .GetLabels(out var labels) // backup branch labels
                .ReplaceWith( // replace with custom quality
                    new(OpCodes.Call,
                        typeof(Utility.Professions).MethodNamed(
                            nameof(Utility.Professions.GetEcologistForageQuality)))
                )
                .AddLabels(labels); // restore backed-up labels
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed while patching automated Berry Bush quality.\nHelper returned {ex}",
                LogLevel.Error);
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches
}