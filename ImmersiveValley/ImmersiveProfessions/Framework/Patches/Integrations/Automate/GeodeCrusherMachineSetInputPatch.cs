namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common.Extensions.Reflection;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class GeodeCrusherMachineSetInputPatch : BasePatch
{
    private static MethodInfo _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal GeodeCrusherMachineSetInputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.GeodeCrusherMachine".ToType().RequireMethod("SetInput");
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

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
        var machine = (SObject) _GetMachine.Invoke(__instance, null);
        if (machine?.heldObject.Value is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Gemologist) ||
            !machine.heldObject.Value.IsForagedMineral() && !machine.heldObject.Value.IsGemOrMineral()) return;

        machine.heldObject.Value.Quality = owner.GetGemologistMineralQuality();
        if (!ModEntry.Config.ShouldCountAutomatedHarvests) return;

        owner.IncrementData<uint>(ModData.GemologistMineralsCollected);
    }

    #endregion harmony patches
}