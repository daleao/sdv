namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common;
using DaLion.Common.Data;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class MushroomBoxMachineGetOutputPatch : BasePatch
{
    private delegate SObject GetMachineDelegate(object instance);

    private static GetMachineDelegate _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal MushroomBoxMachineGetOutputPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.MushroomBoxMachine".ToType()
                .RequireMethod("GetOutput");
        }
        catch
        {
            // ignored
        }

        Prefix!.priority = Priority.HigherThanNormal;
    }

    #region harmony patches

    /// <summary>Patch for automated Mushroom Box forage increment.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static void MushroomBoxMachineGetOutputPrefix(object __instance)
    {
        try
        {
            if (__instance is null) return;

            _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine").CreateDelegate<GetMachineDelegate>();
            var machine = _GetMachine(__instance);
            if (machine.heldObject.Value is null) return;

            var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
            if (!owner.HasProfession(Profession.Ecologist) || !ModEntry.Config.ShouldCountAutomatedHarvests)
                return;

            ModDataIO.IncrementData<uint>(owner, ModData.EcologistItemsForaged.ToString());
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
        }
    }

    #endregion harmony patches
}