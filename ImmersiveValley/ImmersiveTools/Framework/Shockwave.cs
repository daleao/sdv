namespace DaLion.Stardew.Tools.Framework.Effects;

#region using directives

using Common;
using Common.Classes;
using Extensions;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Spreads a tool's effect across all tiles in a circular area.</summary>
internal class Shockwave
{
    private const int SHOCKWAVE_DELAY_MS_I = 150;

    private readonly IEffect _effect;
    private readonly Tool _tool;
    private readonly Vector2 _epicenter;
    private readonly GameLocation _location;
    private readonly Farmer _farmer;
    private readonly List<CircleTileGrid> _tileGrids = new();
    private readonly double _millisecondsWhenReleased;
    private readonly int _finalRadius;
    private int _currentRadius = 1;

    /// <summary>Construct an instance.</summary>
    /// <param name="radius">The maximum radius of the shockwave.</param>
    /// <param name="who">The player who initiated the shockwave.</param>
    /// <param name="milliseconds">The total elapsed <see cref="GameTime"/> in milliseconds at the moment the tool was released.</param>
    internal Shockwave(int radius, Farmer who, double milliseconds)
    {
        _farmer = who;
        _location = who.currentLocation;
        _tool = who.CurrentTool;
#pragma warning disable CS8509
        _effect = _tool switch
#pragma warning restore CS8509
        {
            Axe => new AxeEffect(ModEntry.Config.AxeConfig),
            Pickaxe => new PickaxeEffect(ModEntry.Config.PickaxeConfig)
        };

        _epicenter = new((int)(_farmer.GetToolLocation().X / Game1.tileSize),
            (int)(_farmer.GetToolLocation().Y / Game1.tileSize));
        _finalRadius = radius;

        if (ModEntry.Config.TicksBetweenWaves <= 0)
        {
            _tileGrids.Add(new(_epicenter, _finalRadius));
            _currentRadius = _finalRadius;
        }
        else
        {
            for (var i = 0; i < _finalRadius; ++i) _tileGrids.Add(new(_epicenter, i + 1));
        }

        _millisecondsWhenReleased = milliseconds;
    }

    /// <summary>Expand the affected radius by one unit and apply the tool's effects.</summary>
    /// <param name="milliseconds">The current elapsed <see cref="GameTime"/> in milliseconds.</param>
    internal void Update(double milliseconds)
    {
        if (milliseconds - _millisecondsWhenReleased < SHOCKWAVE_DELAY_MS_I) return;

        IEnumerable<Vector2> affectedTiles;
        if (_tileGrids.Count > 1)
        {
            affectedTiles = _tileGrids[_currentRadius - 1].Tiles;
            if (_currentRadius > 1) affectedTiles = affectedTiles.Except(_tileGrids[_currentRadius - 2].Tiles);
        }
        else
        {
            affectedTiles = _tileGrids[0].Tiles;
        }

        foreach (var tile in affectedTiles.Except(new[] { _epicenter, _farmer.getTileLocation() }))
        {
            _farmer.TemporarilyFakeInteraction(() =>
            {
                // face tile to avoid game skipping interaction
                GetRadialAdjacentTile(_epicenter, tile, out var adjacentTile, out var facingDirection);
                _farmer.Position = adjacentTile * Game1.tileSize;
                _farmer.FacingDirection = facingDirection;

                // apply tool effects
                _location.objects.TryGetValue(tile, out var tileObj);
                _location.terrainFeatures.TryGetValue(tile, out var tileFeature);
                _effect.Apply(tile, tileObj, tileFeature, _tool, _location, _farmer);
            });

            var pixelPos = new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize);

            if (_tool is Axe && !ModEntry.Config.AxeConfig.PlayShockwaveAnimation ||
                _tool is Pickaxe && !ModEntry.Config.PickaxeConfig.PlayShockwaveAnimation) continue;

            _location.temporarySprites.Add(new(12, pixelPos, Color.White, 8,
                Game1.random.NextDouble() < 0.5, 50f));
            _location.temporarySprites.Add(new(6, pixelPos, Color.White, 8,
                Game1.random.NextDouble() < 0.5, 30f));
        }

        if (_currentRadius++ < _finalRadius) return;

        if (ModEntry.Config.EnableDebug) Log.D(_tileGrids[^1].ToString());

        ModEntry.Shockwave.Value = null;
    }

    #region private methods

    /// <summary>Get the tile coordinate which is adjacent to the given <paramref name="tile" /> along a radial line from the player.</summary>
    /// <param name="epicenter">The tile containing the player.</param>
    /// <param name="tile">The tile to face.</param>
    /// <param name="adjacent">The tile radially adjacent to the <paramref name="tile" />.</param>
    /// <param name="facingDirection">The direction to face.</param>
    private static void GetRadialAdjacentTile(Vector2 epicenter, Vector2 tile, out Vector2 adjacent, out int facingDirection)
    {
        facingDirection = Utility.getDirectionFromChange(tile, epicenter);
        adjacent = facingDirection switch
        {
            Game1.up => new(tile.X, tile.Y + 1),
            Game1.down => new(tile.X, tile.Y - 1),
            Game1.left => new(tile.X + 1, tile.Y),
            Game1.right => new(tile.X - 1, tile.Y),
            _ => tile
        };
    }

    #endregion private methods
}