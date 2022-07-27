namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Attributes;
using Common.Extensions.Reflection;
using Common.Extensions.Stardew;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly, RequiresMod("Pathoschild.Automate")]
internal sealed class TapperMachineResetPatch : Common.Harmony.HarmonyPatch
{
    private static Func<object, SObject>? _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal TapperMachineResetPatch()
    {
        Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.TapperMachine".ToType()
            .RequireMethod("Reset");
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated tappers.</summary>
    [HarmonyPostfix]
    private static void TapperMachineResetPostfix(object __instance)
    {
        if (!ModEntry.Config.TappersRewardExp) return;

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine")
            .CompileUnboundDelegate<Func<object, SObject>>();
        _GetMachine(__instance).GetOwner().gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}