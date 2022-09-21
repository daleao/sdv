namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using System;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Pathoschild.Automate")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Integration patch.")]
internal sealed class BushMachineOnOutputReducedPatch : HarmonyPatch
{
    private static Func<object, Bush>? _getMachine;

    /// <summary>Initializes a new instance of the <see cref="BushMachineOnOutputReducedPatch"/> class.</summary>
    internal BushMachineOnOutputReducedPatch()
    {
        this.Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine"
            .ToType()
            .RequireMethod("OnOutputReduced");
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated berry bushes.</summary>
    [HarmonyPostfix]
    private static void BushMachineOnOutputReducedPostfix(object __instance)
    {
        if (!ModEntry.Config.BerryBushesRewardExp)
        {
            return;
        }

        _getMachine ??= __instance
            .GetType()
            .RequirePropertyGetter("Machine")
            .CompileUnboundDelegate<Func<object, Bush>>();
        var machine = _getMachine(__instance);
        if (machine.size.Value >= Bush.greenTeaBush)
        {
            return;
        }

        Game1.MasterPlayer.gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}
