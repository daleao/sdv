﻿namespace DaLion.Ponds.Framework.Patchers.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using DaLion.Shared.Reflection;
using HarmonyLib;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
[ModRequirement("Pathoschild.Automate")]
internal sealed class FishPondMachineOnOutputTakenPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondMachineOnOutputTakenPatcher"/> class.</summary>
            /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishPondMachineOnOutputTakenPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Buildings.FishPondMachine"
            .ToType()
            .RequireMethod("OnOutputTaken");
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FishPondMachineOnOutputTakenPostfix(object __instance)
    {
        var machine = Reflector.GetUnboundPropertyGetter<object, FishPond>(__instance, "Machine").Invoke(__instance);
        var held = machine.DeserializeHeldItems();
        if (held.Count == 0)
        {
            return;
        }

        machine.output.Value = held.First();
        var serialized = held
            .Skip(1)
            .Select(i => $"{i.QualifiedItemId},{i.Stack},{((SObject)i).Quality}");
        Data.Write(machine, DataKeys.ItemsHeld, string.Join(';', serialized));
    }

    #endregion harmony patches
}
