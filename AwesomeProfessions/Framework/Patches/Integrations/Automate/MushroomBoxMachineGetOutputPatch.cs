namespace DaLion.Stardew.Professions.Framework.Patches.Integrations;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class MushroomBoxMachineGetOutputPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal MushroomBoxMachineGetOutputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine".ToType()
                .MethodNamed("GetOutput");
        }
        catch
        {
            // ignored
        }

        Prefix.after = new[] {"Goldenrevolver.ForageFantasy"};
    }

    #region harmony patches

    /// <summary>Patch for automated Mushroom Box quality and forage increment.</summary>
    [HarmonyPrefix]
    private static bool MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        try
        {
            if (__instance is null) return true; // run original logic

            var machine = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
            if (machine?.heldObject.Value is null) return true; // run original logic

            var who = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
            if (!who.HasProfession(Profession.Ecologist)) return true; // run original logic

            machine.heldObject.Value.Quality = who.GetEcologistForageQuality();
            if (!ModEntry.Config.ShouldCountAutomatedHarvests) return true; // run original logic

            ModData.Increment<uint>(DataField.EcologistItemsForaged, who);
            return true; // run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}