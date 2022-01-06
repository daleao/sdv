using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches.Compatibility.Automate;

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

        Prefix.after = new[] { "Goldenrevolver.ForageFantasy" };
    }

    #region harmony patches

    /// <summary>Patch for automated Mushroom Box quality and forage increment.</summary>
    [HarmonyPrefix]
    private static bool MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        if (__instance is null) return true; // run original logic

        var machine = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
        if (machine?.heldObject.Value is null) return true; // run original logic

        var who = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!who.HasProfession("Ecologist")) return true; // run original logic

        machine.heldObject.Value.Quality = Utility.Professions.GetEcologistForageQuality();
        if (!ModEntry.Config.ShouldCountAutomatedHarvests) return true; // run original logic

        ModData.Increment("ItemsForaged", who.UniqueMultiplayerID);
        return true; // run original logic
    }

    #endregion harmony patches
}