namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MonsterShouldActuallyMoveAwayFromPlayerPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MonsterShouldActuallyMoveAwayFromPlayerPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal MonsterShouldActuallyMoveAwayFromPlayerPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target =
            this.RequireMethod<Monster>(nameof(Monster.ShouldActuallyMoveAwayFromPlayer));
    }

    #region harmony patches

    /// <summary>Implement fear status.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MonsterShouldActuallyMoveAwayFromPlayerPrefix(Monster __instance, ref bool __result)
    {
        if (!__instance.IsFeared())
        {
            return true; // run original logic
        }

        __result = true;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
