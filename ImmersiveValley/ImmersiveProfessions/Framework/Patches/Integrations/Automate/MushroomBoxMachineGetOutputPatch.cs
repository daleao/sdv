namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common.Extensions.Reflection;
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
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine".ToType().RequireMethod("GetOutput");
        }
        catch
        {
            // ignored
        }

        Prefix.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Patch for automated Mushroom Box forage increment.</summary>
    [HarmonyPrefix]
    private static void MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        try
        {
            if (__instance is null) return;

            _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
            var machine = (SObject) _GetMachine.Invoke(__instance, null);
            if (machine?.heldObject.Value is null) return;

            var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
            if (!owner.HasProfession(Profession.Ecologist) || !ModEntry.Config.ShouldCountAutomatedHarvests)
                return;

            owner.IncrementData<uint>(DataField.EcologistItemsForaged);
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}