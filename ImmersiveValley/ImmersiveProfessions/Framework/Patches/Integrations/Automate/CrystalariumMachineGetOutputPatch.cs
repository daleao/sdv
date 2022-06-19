﻿namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.Automate;

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
internal sealed class CrystalariumMachineGetOutputPatch : BasePatch
{
    [CanBeNull] private static MethodInfo _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal CrystalariumMachineGetOutputPatch()
    {
        try
        {
            Original = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.CrystalariumMachine".ToType().RequireMethod("GetOutput");
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

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine");
        var machine = (SObject) _GetMachine!.Invoke(__instance, null);
        if (machine?.heldObject.Value is null) return;

        var owner = Game1.getFarmerMaybeOffline(machine.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Gemologist) ||
            !machine.heldObject.Value.IsForagedMineral() && !machine.heldObject.Value.IsGemOrMineral()) return;

        owner.IncrementData<uint>(ModData.GemologistMineralsCollected);
    }

    #endregion harmony patches
}