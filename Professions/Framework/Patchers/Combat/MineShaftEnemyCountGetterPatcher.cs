namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class MineShaftEnemyCountGetterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftEnemyCountGetterPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MineShaftEnemyCountGetterPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequirePropertyGetter<MineShaft>(nameof(MineShaft.EnemyCount));
    }

    #region harmony patches

    /// <summary>Patch to ignore ally Slimes.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MineShaftEnemyCountGetterPrefix(MineShaft __instance, ref int __result)
    {
        __result = __instance.characters.Count(c => c is Monster && (c is not GreenSlime slime || !slime.IsPiped()));
        return false; // don't run original logic
    }

    #endregion harmony patches
}
