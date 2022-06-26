namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.MushroomPropagator;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common.Data;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class PropagatorMachineGetOutputPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static Func<object, SObject>? _GetEntity;

    /// <summary>Construct an instance.</summary>
    internal PropagatorMachineGetOutputPatch()
    {
        try
        {
            Target = "BlueberryMushroomAutomation.PropagatorMachine".ToType().RequireMethod("GetOutput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch for automated Propagator forage decrement.</summary>
    [HarmonyPostfix]
    private static void PropagatorMachineGetOutputPostfix(object __instance)
    {
        _GetEntity ??= __instance.GetType().RequirePropertyGetter("Entity")
            .CompileUnboundDelegate<Func<object, SObject>>();
        var entity = _GetEntity(__instance);
        var owner = Game1.getFarmerMaybeOffline(entity.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Ecologist)) return;

        if (owner.IsLocalPlayer && !ModEntry.Config.ShouldCountAutomatedHarvests)
            ModDataIO.IncrementData(Game1.player, ModData.EcologistItemsForaged.ToString(), -1);
        else if (ModEntry.Config.ShouldCountAutomatedHarvests)
            ModDataIO.IncrementData<uint>(owner, ModData.EcologistItemsForaged.ToString());
    }

    #endregion harmony patches
}