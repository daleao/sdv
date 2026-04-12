namespace DaLion.Professions.Framework.Patchers.Mining;

#region using dependencies

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;
using StardewValley.Monsters;

#endregion using dependencies

[UsedImplicitly]
internal sealed class MineShaftEnemyCountPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MineShaftEnemyCountPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MineShaftEnemyCountPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = AccessTools.DeclaredProperty(typeof(MineShaft), nameof(MineShaft.EnemyCount)).GetGetMethod();
    }

    #region harmony patches

    /// <summary>Patch to filter summoned slimes from a mine's enemy count.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool MineShaftEnemyCountPrefix(MineShaft __instance, ref int __result)
    {
        bool IsEnemyMonster(NPC p) => p is Monster && (p is not GreenSlime slime || slime.Get_Piped() == null);
        __result = __instance.characters.Count(IsEnemyMonster);
        return false;
    }

    #endregion harmony patches
}
