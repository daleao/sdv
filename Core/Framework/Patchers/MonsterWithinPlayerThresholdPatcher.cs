namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterWithinPlayerThresholdPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterWithinPlayerThresholdPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MonsterWithinPlayerThresholdPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target =
            this.RequireMethod<Monster>(nameof(Monster.withinPlayerThreshold));
    }

    #region harmony patches

    /// <summary>Implement fear status.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MonsterShouldActuallyMoveAwayFromPlayerPrefix(Monster __instance, ref bool __result)
    {
        if (!__instance.IsBlinded())
        {
            return true; // run original logic
        }

        __result = __instance.withinPlayerThreshold(__instance.moveTowardPlayerThreshold.Value / 4);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
