namespace DaLion.Professions.Framework;

#region using directives

using DaLion.Core.Framework;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;

#endregion using directives

/// <summary>A <see cref="GreenSlime"/> under influence of <see cref="PiperConcerto"/>.</summary>
internal sealed class PipedSlime : IDisposable
{
    internal const CollisionMask COLLISION_MASK = CollisionMask.Buildings | CollisionMask.Furniture |
                                                 CollisionMask.Objects | CollisionMask.TerrainFeatures |
                                                 CollisionMask.LocationSpecific;

    private const int FADE_IN_DURATION = 60;
#if RELEASE
    private const int RESPAWN_TIME = 42000;
#elif DEBUG
    private const int RESPAWN_TIME = 7000;
#endif

    private readonly NetRef<Hat?> _hat = [];
    private int _respawnTimer = -1;
    private int _fadeInCounter = -1;

    /// <summary>Initializes a new instance of the <see cref="PipedSlime"/> class.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/> instance.</param>
    /// <param name="piper">The <see cref="Farmer"/> who cast <see cref="PiperConcerto"/>.</param>
    /// <paramref name="summoned">Whether the instance is a temporary Concerto Slime.</paramref>
    internal PipedSlime(GreenSlime slime, Farmer piper, bool summoned = true)
    {
        this.Slime = slime;
        this.Piper = piper;
        this.IsSummoned = summoned;
        this.OriginalHealth = slime.MaxHealth;
        this.OriginalDamage = slime.DamageToFarmer;
        this.OriginalSpeed = slime.Speed;
        this.OriginalRange = slime.moveTowardPlayerThreshold.Value;
        this.OriginalScale = slime.Scale;

        this.FakeFarmer = new FakeFarmer
        {
            UniqueMultiplayerID = slime.GetHashCode(), currentLocation = piper.currentLocation,
        };

        slime.Health = slime.MaxHealth;
        slime.farmerPassesThrough = true;
        Reflector.GetUnboundFieldSetter<GreenSlime, bool>(slime, "ignoreMovementAnimations").Invoke(slime, false);
        if (Config.UseAsyncMinionPathfinder)
        {
            (PathfinderAsync ??= new PathfindingManagerAsync(
                EventManager,
                (l, t) => l.isTilePassable(t) && (!l.IsTileOccupiedBy(t, COLLISION_MASK))))
                .Register(slime, slime.currentLocation)
                .QueueRequest(slime.TilePoint, piper.TilePoint);
        }
        else
        {
            (Pathfinder ??= new PathfindingManager(
                    EventManager,
                    (l, t) => l.isTilePassable(t) && (!l.IsTileOccupiedBy(t, COLLISION_MASK))))
                .Register(slime, slime.currentLocation)
                .RequestFor(slime.TilePoint, piper.TilePoint);
            Pathfinder.Debug(slime);
        }
    }

    /// <summary>Gets the <see cref="GreenSlime"/> instance.</summary>
    internal GreenSlime Slime { get; }

    /// <summary>Gets the <see cref="Farmer"/> who cast <see cref="PiperConcerto"/>.</summary>
    internal Farmer Piper { get; }

    /// <summary>Gets the alpha value for drawing the <seealso cref="Slime"/>.</summary>
    internal float Alpha => this._fadeInCounter >= 0
        ? (float)((-1d / (1d + Math.Exp((12d * this._fadeInCounter / FADE_IN_DURATION) - 6d))) + 1d)
        : 1f;

    /// <summary>Gets the fake <see cref="Farmer"/> instance used for aggroing onto non-players.</summary>
    internal FakeFarmer FakeFarmer { get; }

    /// <summary>Gets the instance's <see cref="Inventory"/>.</summary>
    internal Inventory Inventory { get; } = [];

    /// <summary>Gets the original health of the instance, before it was inflated.</summary>
    internal int OriginalHealth { get; }

    /// <summary>Gets the original damage of the instance, before it was inflated.</summary>
    internal int OriginalDamage { get; }

    /// <summary>Gets the original movement speed of the instance, before it was inflated.</summary>
    internal int OriginalSpeed { get; }

    /// <summary>Gets the original aggro range of the instance, before it was inflated.</summary>
    internal int OriginalRange { get; }

    /// <summary>Gets the original scale of the instance, before it was inflated.</summary>
    internal float OriginalScale { get; }

    /// <summary>Gets a value indicating whether the instance has been fully inflated.</summary>
    internal bool DoneInflating { get; private set; }

    /// <summary>Gets a value indicating whether the instance is a raised Slime or a temporary Concerto Slime.</summary>
    internal bool IsSummoned { get; }

    /// <summary>Gets or sets the currently worn hat.</summary>
    internal Hat? Hat
    {
        get => this._hat.Value;
        set
        {
            if (this.Inventory.Count == 0)
            {
                for (var i = 0; i < 12; i++)
                {
                    this.Inventory.Add(null);
                }
            }

            this._hat.Value = value;
        }
    }

    /// <summary>Gets a value indicating whether the Piped Slime's <seealso cref="Inventory"/> contains any items.</summary>
    internal bool IsCarryingItems => this.Hat is not null && this.Inventory.WhereNotNull().Any();

    /// <summary>Gets a value indicating whether the Piped Slime's <seealso cref="Inventory"/> has any empty slots.</summary>
    internal bool HasEmptyInventorySlots
    {
        get
        {
            if (this.Hat is null)
            {
                return false;
            }

            Utility.consolidateStacks(this.Inventory);
            while (this.Inventory.Count < 12)
            {
                this.Inventory.Add(null);
            }

            return this.Inventory.HasEmptySlots() || this.Inventory.HasEmptySlots();
        }
    }

    /// <summary>Gets a value indicating whether the <seealso cref="Slime"/> is currently dead and waiting to respawn.</summary>
    internal bool IsRespawning => this.Slime.Health <= 0 && this._respawnTimer >= 0;

    /// <summary>Checks for actions on the instance.</summary>
    /// <param name="who">The <see cref="Farmer"/> who is checking.</param>
    public void CheckAction(Farmer who)
    {
        if (!ReferenceEquals(who, this.Piper))
        {
            return;
        }

        if (who.Items.Count > who.CurrentToolIndex && who.Items[who.CurrentToolIndex] is Hat hat)
        {
            if (this.Hat is not null)
            {
                var slime = this.Slime;
                Game1.createItemDebris(this.Hat, slime.Position, slime.FacingDirection);
                this.Hat = null;

                var currentLocation = slime.currentLocation;
                var inDangerZone = currentLocation.IsEnemyArea() ||
                                     currentLocation.Name.ContainsAnyOf("Mine", "SkullCave");
                if (!inDangerZone && currentLocation.IsOutdoors)
                {
                    slime.Set_Piped(null);
                }
            }
            else
            {
                who.Items[who.CurrentToolIndex] = null;
                this.Hat = hat;
                Game1.playSound("dirtyHit");
            }

            return;
        }

        if (this.Hat is null)
        {
            return;
        }

        if (this.IsCarryingItems)
        {
            this.OpenInventory();
        }
    }

    /// <summary>Draws the <seealso cref="Hat"/>.</summary>
    public void DrawHat(SpriteBatch b)
    {
        if (this.Hat is null)
        {
            return;
        }

        var slime = this.Slime;
        var bounceOffset = slime.Sprite.CurrentFrame switch
        {
            0 => -2,
            2 => 1,
            _ => 0,
        };
        var yOffset = slime.GetBoundingBox().Height + slime.yOffset + bounceOffset +
                      slime.FacingDirection switch
                      {
                          Game1.up => 4,
                          Game1.left or Game1.right => -4,
                          Game1.down => -2,
                      };
        this.Hat.draw(
            b,
            Utility.snapDrawPosition(slime.getLocalPosition(Game1.viewport) - new Vector2(4f, yOffset)),
            1.3333334f,
            1f,
            (slime.StandingPixel.Y / 10000f) + 1E-07f,
            slime.FacingDirection);
    }

    /// <summary>Updates the instance state and counts down necessary timers.</summary>
    /// <param name="time">The current <see cref="GameTime"/>.</param>
    public void Update(GameTime time)
    {
        if (this._respawnTimer > 0)
        {
            this._respawnTimer -= time.ElapsedGameTime.Milliseconds;
            if (this._respawnTimer <= 0)
            {
                this.Slime.Health = this.Slime.MaxHealth;
                this.WarpToPiper();
                this._fadeInCounter = 0;
            }
        }

        if (this._fadeInCounter >= 0)
        {
            this._fadeInCounter++;
            if (this._fadeInCounter >= FADE_IN_DURATION)
            {
                this._fadeInCounter = -1;
            }
        }
    }

    /// <summary>Unregisters from Pathfinders.</summary>
    public void Dispose()
    {
        Pathfinder?.Unregister(this.Slime);
        PathfinderAsync?.Unregister(this.Slime);
    }

    /// <summary>Grows the <see cref="PipedSlime"/> one stage.</summary>
    internal void Inflate()
    {
        var slime = this.Slime;
        slime.Scale = Math.Min(slime.Scale * 1.1f, Math.Min(this.OriginalScale * 2f, 2f));
        if (slime.Scale <= 1.4f ||
            (slime.Scale < this.OriginalScale * 2f &&
             !Game1.random.NextBool(0.2 - (Game1.player.DailyLuck / 2) - (Game1.player.LuckLevel * 0.01))))
        {
            return;
        }

        slime.MaxHealth += (int)Math.Round(slime.Health * slime.Scale * slime.Scale);
        slime.Health = slime.MaxHealth;
        slime.DamageToFarmer = (int)(slime.DamageToFarmer * slime.Scale);
        slime.moveTowardPlayerThreshold.Value = 9999;
        if (Game1.random.NextBool(1d / 3d))
        {
            slime.addedSpeed += Game1.random.Next(3);
        }

        if (slime.Scale >= 1.8f)
        {
            slime.willDestroyObjectsUnderfoot = true;
        }

        this.DoneInflating = true;
    }

    /// <summary>Shrinks this <see cref="Piper"/> one stage.</summary>
    internal void Deflate()
    {
        var slime = this.Slime;
        slime.Scale = Math.Max(slime.Scale / 1.1f, this.OriginalScale);
        if (slime.Scale > this.OriginalScale)
        {
            return;
        }

        slime.MaxHealth = this.OriginalHealth;
        slime.Health = slime.MaxHealth;
        slime.DamageToFarmer = this.OriginalDamage;
        slime.moveTowardPlayerThreshold.Value = this.OriginalRange;
        slime.willDestroyObjectsUnderfoot = false;
        slime.addedSpeed = 0;
        slime.focusedOnFarmers = false;
        this.DoneInflating = false;
    }

    /// <summary>Collects the <see cref="Item"/> contained within the <paramref name="debris"/>.</summary>
    /// <param name="debris">The <see cref="Debris"/>.</param>
    /// <returns><see langword="true"/> if the <see cref="Debris"/> was successfully collected, otherwise <see langword="false"/>.</returns>
    internal bool TryCollectDebris(Debris debris)
    {
        Item? debrisItem;
        if (debris.item is not null)
        {
            debrisItem = debris.item;
            debris.item = null;
        }
        else if (!string.IsNullOrEmpty(debris.itemId.Value))
        {
            debrisItem = ItemRegistry.Create(debris.itemId.Value, 1, debris.itemQuality);
        }
        else
        {
            return false;
        }

        foreach (var ownedItem in this.Piper.Items)
        {
            if (ownedItem is null || !ownedItem.canStackWith(debrisItem))
            {
                continue;
            }

            var toRemove = debrisItem.Stack - ownedItem.addToStack(debrisItem);
            if (debrisItem.ConsumeStack(toRemove) == null)
            {
                return true;
            }
        }

        for (var i = 0; i < this.Inventory.Count; i++)
        {
            var pipedItem = this.Inventory[i];
            if (pipedItem is not null)
            {
                if (pipedItem.canStackWith(debrisItem))
                {
                    var toRemove = debrisItem.Stack - pipedItem.addToStack(debrisItem);
                    if (debrisItem.ConsumeStack(toRemove) == null)
                    {
                        return true;
                    }
                }
                else
                {
                    continue;
                }
            }

            this.Inventory[i] = debrisItem;
            return true;
        }

        debris.item = debrisItem;
        return false;
    }

    /// <summary>Initiates the respawn countdown.</summary>
    internal void BeginRespawn()
    {
        this._respawnTimer = RESPAWN_TIME;
    }

    /// <summary>Warps the <seealso cref="Slime"/> to a new position near its <seealso cref="Piper"/> and resets the AI state.</summary>
    internal void WarpToPiper()
    {
        var slime = this.Slime;
        var piper = this.Piper;
        var piperLocation = piper.currentLocation;
        var spawnTile = piper.CountPipedSlimes() < 8
            ? piper.ChooseFromEightNeighboringTiles((tile, location) => location.CanSpawnCharacterHere(tile))
            : piper.ChooseFromTwentyFourNeighboringTiles((tile, location) => location.CanSpawnCharacterHere(tile));
        Game1.warpCharacter(slime, piperLocation, spawnTile);
        this.FakeFarmer.currentLocation = piperLocation;
        this.FakeFarmer.Position = spawnTile * Game1.tileSize;
        if (Config.UseAsyncMinionPathfinder)
        {
            (PathfinderAsync ??= new PathfindingManagerAsync(
                    EventManager,
                    (l, t) => l.isTilePassable(t) && (!l.IsTileOccupiedBy(t, COLLISION_MASK))))
                .Reregister(slime, slime.currentLocation)
                .QueueRequest(slime.TilePoint, piper.TilePoint);
        }
        else
        {
            (Pathfinder ??= new PathfindingManager(
                    EventManager,
                    (l, t) => l.isTilePassable(t) && (!l.IsTileOccupiedBy(t, COLLISION_MASK))))
                .Reregister(slime, slime.currentLocation)
                .RequestFor(slime.TilePoint, piper.TilePoint);
            Pathfinder.Debug(slime);
        }

        Log.D($"Warped to {piperLocation}...");
    }

    /// <summary>Burst.</summary>
    internal void Burst()
    {
        var slime = this.Slime;
        if (this._respawnTimer >= 0)
        {
            this._respawnTimer = -1;
            slime.Set_Piped(null);
            return;
        }

        if (this.IsCarryingItems)
        {
            this.DropItems();
        }

        slime.Health = 0;
        slime.deathAnimation();
        slime.Set_Piped(null);
    }

    /// <summary>Reset to original stats.</summary>
    internal void Reset()
    {
        var slime = this.Slime;
        slime.MaxHealth = this.OriginalHealth;
        slime.Health = Math.Min(slime.Health, slime.MaxHealth);
        slime.DamageToFarmer = this.OriginalDamage;
        slime.Speed = this.OriginalSpeed;
        slime.moveTowardPlayerThreshold.Value = this.OriginalRange;
        slime.Scale = this.OriginalScale;
    }

    /// <summary>Opens the Piped Slime's <seealso cref="Inventory"/>.</summary>
    private void OpenInventory()
    {
        if (this.Inventory.Count == 0)
        {
            return;
        }

        var menu = new ItemGrabMenu(this.Inventory, this).setEssential(false);
        menu.source = ItemGrabMenu.source_none;
        Game1.activeClickableMenu = menu;
    }

    /// <summary>Release the items held by this instance.</summary>
    private void DropItems()
    {
        foreach (var item in this.Inventory)
        {
            if (item is not null)
            {
                Game1.createItemDebris(
                    item,
                    this.Slime.Position,
                    Game1.random.Next(4),
                    this.Slime.currentLocation);
            }
        }
    }
}
