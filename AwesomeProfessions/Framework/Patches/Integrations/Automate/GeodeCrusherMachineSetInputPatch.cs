namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Stardew.Common.Harmony;
using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class GeodeCrusherMachineSetInputPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GeodeCrusherMachineSetInputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.GeodeCrusherMachine".ToType()
                .MethodNamed("SetInput");
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

        var machine = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
        if (machine?.heldObject.Value is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Gemologist) ||
            !machine.heldObject.Value.IsForagedMineral() && !machine.heldObject.Value.IsGemOrMineral()) return;

        machine.heldObject.Value.Quality = owner.GetGemologistMineralQuality();
        if (!ModEntry.Config.ShouldCountAutomatedHarvests) return;

        ModData.Increment<uint>(DataField.GemologistMineralsCollected, owner);
    }

    #endregion harmony patches
}