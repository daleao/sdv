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
internal sealed class CrystalariumMachineGetOutputPatch : BasePatch
{
    private delegate SObject GetMachineDelegate(object instance);

    private static GetMachineDelegate _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal CrystalariumMachineGetOutputPatch()
    {
        try
        {
            Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrystalariumMachine".ToType()
                .RequireMethod("GetOutput");
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

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine").CreateDelegate<GetMachineDelegate>();
        var machine = _GetMachine(__instance);
        if (machine.heldObject.Value is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Gemologist) ||
            !machine.heldObject.Value.IsForagedMineral() && !machine.heldObject.Value.IsGemOrMineral()) return;

        ModDataIO.IncrementData<uint>(owner, ModData.GemologistMineralsCollected.ToString());
    }

    #endregion harmony patches
}