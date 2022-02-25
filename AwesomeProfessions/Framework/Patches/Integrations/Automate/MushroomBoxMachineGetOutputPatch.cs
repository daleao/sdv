namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Extensions;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class MushroomBoxMachineGetOutputPatch : BasePatch
{
    private static MethodInfo _GetMachine;

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

            _GetMachine ??= __instance.GetType().PropertyGetter("Machine");
            var machine = (SObject) _GetMachine.Invoke(__instance, null);
            if (machine?.heldObject.Value is null) return true; // run original logic

            var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
            if (!owner.HasProfession(Profession.Ecologist)) return true; // run original logic

            machine.heldObject.Value.Quality = owner.GetEcologistForageQuality();
            if (!ModEntry.Config.ShouldCountAutomatedHarvests) return true; // run original logic

            owner.IncrementData<uint>(DataField.EcologistItemsForaged);
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