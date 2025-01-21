namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationMonsterDropPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationMonsterDropPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationMonsterDropPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.monsterDrop));
    }

    #region harmony patches

    /// <summary>Patch to do Prestiged Slime drops.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GameLocationMonsterDropPostfix(
        GameLocation __instance, Monster monster, int x, int y)
    {
        foreach (var (slime, _) in GreenSlime_Piped.Values)
        {
            if (ReferenceEquals(slime.currentLocation, __instance) && slime.prismatic.Value &&
                slime.TileDistanceTo(monster) <= 10 && Game1.random.NextBool(0.05))
            {
                Game1.createObjectDebris(QIDs.PrismaticShard, x, y, __instance);
            }
        }
    }

    #endregion harmony patches
}
