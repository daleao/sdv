namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using Common.Attributes;
using Common.Extensions.Reflection;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using System;

#endregion using directives

[UsedImplicitly, RequiresMod("Pathoschild.Automate")]
internal sealed class BushMachineOnOutputReducedPatch : Common.Harmony.HarmonyPatch
{
    private static Func<object, Bush>? _GetMachine;

    /// <summary>Construct an instance.</summary>
    internal BushMachineOnOutputReducedPatch()
    {
        Target = "Pathoschild.Stardew.Automate.Framework.Machines.TerrainFeatures.BushMachine".ToType()
            .RequireMethod("OnOutputReduced");
    }

    #region harmony patches

    /// <summary>Adds foraging experience for automated berry bushes.</summary>
    [HarmonyPostfix]
    private static void BushMachineOnOutputReducedPostfix(object __instance)
    {
        if (!ModEntry.Config.BerryBushesRewardExp) return;

        _GetMachine ??= __instance.GetType().RequirePropertyGetter("Machine")
            .CompileUnboundDelegate<Func<object, Bush>>();
        var machine = _GetMachine(__instance);
        if (machine.size.Value >= Bush.greenTeaBush) return;

        Game1.MasterPlayer.gainExperience(Farmer.foragingSkill, 5);
    }

    #endregion harmony patches
}