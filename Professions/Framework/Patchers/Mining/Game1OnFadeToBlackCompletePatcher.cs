namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Extensions.Xna;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class Game1OnFadeToBlackCompletePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="Game1OnFadeToBlackCompletePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal Game1OnFadeToBlackCompletePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Game1>("onFadeToBlackComplete");
    }

    #region harmony patches

    /// <summary>Patch to implement Prestiged Spelunker checkpoint.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void Game1OnFadeToBlackCompletePostfix()
    {
        if (State.SpelunkerFlag is not { } flag || !State.UsingSpelunkerCheckpoint)
        {
            return;
        }

        var checkpointTile = flag.TileLocation;
        var mapWidth = flag.Location.Map.Layers[0].LayerWidth;
        var mapHeight = flag.Location.Map.Layers[0].LayerHeight;
        var spawnTile = checkpointTile.GetEightNeighbors(mapWidth, mapHeight).Where(t => !flag.Location.Objects.ContainsKey(t)).Choose();
        var checkpointDirection = (checkpointTile - spawnTile).ToFacingDirection();
        Game1.player.Position = new Vector2(checkpointTile.X * Game1.tileSize, checkpointTile.Y * Game1.tileSize);
        Game1.player.faceDirection((int)checkpointDirection);
        State.UsingSpelunkerCheckpoint = false;
    }

    #endregion harmony patches
}
