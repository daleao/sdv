using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Patches.Compatibility.Automate;

[UsedImplicitly]
internal class CrystalariumMachineGetOutputPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal CrystalariumMachineGetOutputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrystalariumMachine".ToType()
                .MethodNamed("GetOutput");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to increment minerals collected from automated Crystalariums.</summary>
    [HarmonyPostfix]
    private static void CrystalariumMachineGetOutputPostfix(object __instance)
    {
        if (__instance is null || !ModEntry.Config.ShouldCountAutomatedHarvests) return;

        var machine = ModEntry.ModHelper.Reflection.GetProperty<SObject>(__instance, "Machine").GetValue();
        if (machine?.heldObject.Value is null) return;

        var who = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!who.IsLocalPlayer || !who.HasProfession("Gemologist") ||
            !machine.heldObject.Value.IsForagedMineral() && !machine.heldObject.Value.IsGemOrMineral()) return;

        ModEntry.Data.Value.Increment<uint>("MineralsCollected");
    }

    #endregion harmony patches
}