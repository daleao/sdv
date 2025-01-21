namespace DaLion.Professions.Framework.Patchers.Combat;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationOnMonsterKilledPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationOnMonsterKilledPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationOnMonsterKilledPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>("onMonsterKilled");
    }

    #region harmony patches

    /// <summary>Patch to do Prestiged Gold Slime drops.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GameLocationMonsterDropPostfix(GameLocation __instance, Monster monster)
    {
        foreach (var (slime, piped) in GreenSlime_Piped.Values)
        {
            if (ReferenceEquals(slime.currentLocation, __instance) && slime.Name == "Gold Slime" &&
                slime.TileDistanceTo(monster) <= 5)
            {
                piped.Piper.Money += 100;
            }
        }
    }

    #endregion harmony patches
}
