namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Locations;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1EnterMinePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1EnterMinePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal Game1EnterMinePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Game1>(nameof(Game1.enterMine));
    }

    #region harmony patches

    /// <summary>Patch to implement Prestiged Spelunker checkpoint.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void Game1EnterMinePrefix()
    {
        if (Game1.currentLocation is not MineShaft shaft || !shaft.IsTreasureOrSafeRoom() ||
            !Game1.player.HasProfession(Profession.Spelunker, true) || State.HasSpelunkerUsedCheckpointToday)
        {
            return;
        }

        State.SpelunkerCheckpoint = shaft;
        State.SpelunkerCheckpointTile = Game1.player.Tile;
        State.SpelunkerCheckpointDirection = Game1.player.FacingDirection;
    }

    #endregion harmony patches
}
