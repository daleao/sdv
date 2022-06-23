namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

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
internal sealed class GeodeCrusherMachineSetInputPatch : BasePatch
{
    private delegate SObject GetMachineDelegate(object instance);

    private static GetMachineDelegate _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal GeodeCrusherMachineSetInputPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.GeodeCrusherMachine".ToType()
                .RequireMethod("SetInput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to apply Gemologist effects to automated Geode Crusher.</summary>
    [HarmonyPostfix]
    private static void GeodeCrusherMachineSetInputPostfix(object __instance)
    {
        if (__instance is null) return;

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine").CreateDelegate<GetMachineDelegate>();
        var machine = _GetMachine(__instance);
        if (machine.heldObject.Value is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Gemologist) ||
            !machine.heldObject.Value.IsForagedMineral() && !machine.heldObject.Value.IsGemOrMineral()) return;

        machine.heldObject.Value.Quality = owner.GetGemologistMineralQuality();
        if (!ModEntry.Config.ShouldCountAutomatedHarvests) return;

        ModDataIO.IncrementData<uint>(owner, ModData.GemologistMineralsCollected.ToString());
    }

    #endregion harmony patches
}