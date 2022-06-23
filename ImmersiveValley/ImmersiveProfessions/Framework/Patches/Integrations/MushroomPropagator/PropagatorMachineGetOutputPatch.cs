namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.MushroomPropagator;

#region using directives

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
internal sealed class PropagatorMachineGetOutputPatch : BasePatch
{
    private delegate SObject GetEntityDelegate(object instance);

    private static GetEntityDelegate _GetEntity;

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
        if (__instance is null) return;

        _GetEntity ??= __instance.GetType().RequirePropertyGetter("Entity").CreateDelegate<GetEntityDelegate>();
        var entity = _GetEntity(__instance);
        if (entity is null) return;

        var owner = Game1.getFarmerMaybeOffline(entity.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Ecologist)) return;

        if (owner.IsLocalPlayer && !ModEntry.Config.ShouldCountAutomatedHarvests)
            ModDataIO.IncrementData(Game1.player, ModData.EcologistItemsForaged.ToString(), -1);
        else if (ModEntry.Config.ShouldCountAutomatedHarvests)
            ModDataIO.IncrementData<uint>(owner, ModData.EcologistItemsForaged.ToString());
    }

    #endregion harmony patches
}