namespace DaLion.Chargeable.Framework;

#region using directives

using System.Collections.Generic;
using System.Linq;
using DaLion.Chargeable.Framework.Effects;
using DaLion.Chargeable.Framework.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>Spreads a tool's effect across all tiles in a circular area.</summary>
internal class Shockwave
{
    private const int ShockwaveDelay = 150;

    private readonly IToolEffect? _effect;
    private readonly Vector2 _epicenter;
    private readonly Farmer _farmer;
    private readonly uint _finalRadius;
    private readonly GameLocation _location;
    private readonly double _millisecondsWhenReleased;
    private readonly List<CircleTileGrid> _tileGrids = new();
    private readonly Tool _tool;
    private uint _currentRadius = 1;

    /// <summary>Initializes a new instance of the <see cref="Shockwave"/> class.</summary>
    /// <param name="radius">The maximum radius of the <see cref="Shockwave"/>.</param>
    /// <param name="who">The player who initiated the <see cref="Shockwave"/>.</param>
    /// <param name="milliseconds">
    ///     The total elapsed <see cref="GameTime"/> in milliseconds at the moment the tool was
    ///     released.
    /// </param>
    internal Shockwave(uint radius, Farmer who, double milliseconds)
    {
        this._farmer = who;
        this._location = who.currentLocation;
        this._tool = who.CurrentTool;
        this._effect = this._tool switch
        {
            Axe => new AxeEffect(Config.Axe),
            Pickaxe => new PickaxeEffect(Config.Pick),
            _ => null,
        };

        this._epicenter = new Vector2(
            (int)(this._farmer.GetToolLocation().X / Game1.tileSize),
            (int)(this._farmer.GetToolLocation().Y / Game1.tileSize));
        this._finalRadius = radius;

        if (Config.TicksBetweenWaves <= 0)
        {
            this._tileGrids.Add(new CircleTileGrid(this._epicenter, this._finalRadius));
            this._currentRadius = this._finalRadius;
        }
        else
        {
            for (uint i = 0; i < this._finalRadius; i++)
            {
                this._tileGrids.Add(new CircleTileGrid(this._epicenter, i + 1));
            }
        }

        this._millisecondsWhenReleased = milliseconds;
    }

    /// <summary>Expands the affected radius by one unit and applies the tool's effects.</summary>
    /// <param name="milliseconds">The current elapsed <see cref="GameTime"/> in milliseconds.</param>
    internal void Update(double milliseconds)
    {
        if (milliseconds - this._millisecondsWhenReleased < ShockwaveDelay)
        {
            return;
        }

        IEnumerable<Vector2> affectedTiles;
        if (this._tileGrids.Count > 1)
        {
            affectedTiles = this._tileGrids[(int)this._currentRadius - 1].Tiles;
            if (this._currentRadius > 1)
            {
                affectedTiles = affectedTiles.Except(this._tileGrids[(int)this._currentRadius - 2].Tiles);
            }
        }
        else
        {
            affectedTiles = this._tileGrids[0].Tiles;
        }

        foreach (var tile in affectedTiles.Except(new[] { this._epicenter, this._farmer.getTileLocation() }))
        {
            this._farmer.TemporarilyFakeInteraction(() =>
            {
                // face tile to avoid game skipping interaction
                GetRadialAdjacentTile(this._epicenter, tile, out var adjacentTile, out var facingDirection);
                this._farmer.Position = adjacentTile * Game1.tileSize;
                this._farmer.FacingDirection = facingDirection;

                // apply tool effects
                this._location.objects.TryGetValue(tile, out var tileObj);
                this._location.terrainFeatures.TryGetValue(tile, out var tileFeature);
                this._effect!.Apply(tile, tileObj, tileFeature, this._tool, this._location, this._farmer);
            });

            var pixelPosition = new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize);

            if ((this._tool is Axe && !Config.Axe.PlayShockwaveAnimation) ||
                (this._tool is Pickaxe && !Config.Pick.PlayShockwaveAnimation))
            {
                continue;
            }

            this._location.temporarySprites.Add(new TemporaryAnimatedSprite(
                12,
                pixelPosition,
                Color.White,
                8,
                Game1.random.NextDouble() < 0.5,
                50f));
            this._location.temporarySprites.Add(new TemporaryAnimatedSprite(
                6,
                pixelPosition,
                Color.White,
                8,
                Game1.random.NextDouble() < 0.5,
                30f));
        }

        if (this._currentRadius++ < this._finalRadius)
        {
            return;
        }

        //Log.D(this._tileGrids[^1].ToString());
        State.Shockwaves.Remove(this);
    }

    /// <summary>
    ///     Gets the tile coordinate which is adjacent to the given <paramref name="tile"/> along a radial line from the
    ///     player.
    /// </summary>
    /// <param name="epicenter">The tile containing the player.</param>
    /// <param name="tile">The tile to face.</param>
    /// <param name="adjacent">The tile radially adjacent to the <paramref name="tile"/>.</param>
    /// <param name="facingDirection">The direction to face.</param>
    private static void GetRadialAdjacentTile(
        Vector2 epicenter, Vector2 tile, out Vector2 adjacent, out int facingDirection)
    {
        facingDirection = Utility.getDirectionFromChange(tile, epicenter);
        adjacent = facingDirection switch
        {
            Game1.up => new Vector2(tile.X, tile.Y + 1),
            Game1.down => new Vector2(tile.X, tile.Y - 1),
            Game1.left => new Vector2(tile.X + 1, tile.Y),
            Game1.right => new Vector2(tile.X - 1, tile.Y),
            _ => tile,
        };
    }
}
