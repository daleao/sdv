namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

#region using directives

using DaLion.Common.Data;
using DaLion.Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class CrystalariumMachineGetOutputPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private static Func<object, SObject>? _GetMachine;

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
        if (!ModEntry.Config.ShouldCountAutomatedHarvests) return;

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine")
            .CompileUnboundDelegate<Func<object, SObject>>();
        var machine = _GetMachine(__instance);
        if (machine.heldObject.Value is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Gemologist) ||
            !machine.heldObject.Value.IsForagedMineral() && !machine.heldObject.Value.IsGemOrMineral()) return;

        ModDataIO.Increment<uint>(owner, "GemologistMineralsCollected");
    }

    #endregion harmony patches
}