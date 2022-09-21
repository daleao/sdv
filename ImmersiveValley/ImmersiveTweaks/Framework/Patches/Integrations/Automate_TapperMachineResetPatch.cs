namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using System;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class TapperMachineResetPatch : HarmonyPatch
{
    private static Func<object, SObject>? _getMachine;

    /// <summary>Initializes a new instance of the <see cref="TapperMachineResetPatch"/> class.</summary>
    internal TapperMachineResetPatch()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.Objects.TapperMachine"
            .ToType()
            .RequireMethod("Reset");
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated tappers.</summary>
    [HarmonyPostfix]
    private static void TapperMachineResetPostfix(object __instance)
    {
        if (!ModEntry.Config.TappersRewardExp)
        {
            return;
        }

        _getMachine ??= __instance
            .GetType()
            .RequirePropertyGetter("Machine")
            .CompileUnboundDelegate<Func<object, SObject>>();
        _getMachine(__instance).GetOwner().gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}
