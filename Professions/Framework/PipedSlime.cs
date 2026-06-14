namespace DaLion.Professions.Framework;

#region using directives

using System.Diagnostics.CodeAnalysis;
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

    private const int FADE_DURATION = 60;
#if RELEASE
    private const int RESPAWN_TIME = 42000;
#elif DEBUG
    private const int RESPAWN_TIME = 7000;
#endif

    private readonly NetRef<Hat?> _hat = [];
    private int _fadeCounter = -1;
    private int _fadeDelta = 0;

    /// <summary>Initializes a new instance of the <see cref="PipedSlime"/> class.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/> instance.</param>
    /// <param name="piper">The <see cref="Farmer"/> who cast <see cref="PiperConcerto"/>.</param>
    /// <param name="source">The <see cref="PipingSource"/> of the piping.</param>
    internal PipedSlime(GreenSlime slime, Farmer piper, PipingSource source)
    {
        this.Slime = slime;
        this.Piper = piper;
        this.Source = source;
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

    /// <summary>Initializes a new instance of the <see cref="PipedSlime"/> class.</summary>
    /// <param name="slime">The <see cref="GreenSlime"/> instance.</param>
    /// <param name="piper">The <see cref="Farmer"/> who cast <see cref="PiperConcerto"/>.</param>
    /// <param name="hat">A <see cref="Hat"/>.</param>
    internal PipedSlime(GreenSlime slime, Farmer piper, Hat hat)
        : this(slime, piper, PipingSource.Hat)
    {
        this.Hat = hat;
    }

    /// <summary>Describes how the slime was piped.</summary>
    internal enum PipingSource
    {
        /// <summary>This Slime was summoned by a Piper.</summary>
        Summoned,

        /// <summary>This Slime was charmed by a Piper.</summary>
        Charmed,

        /// <summary>This Slime was given a hat to wear. The piping lasts indefinitely, or until the Piper removes the Slime's hat.</summary>
        Hat,

        /// <summary>This Slime is being manually herded by a Piper. The piping lasts as long as the Piper holds the Mod Key.</summary>
        Herded,

        /// <summary>This Slime is not Piped.</summary>
        None = -1,
    }

    /// <summary>Gets a the Hat Slime instance.</summary>
    internal static PipedSlime? HatSlime { get; private set; }

    /// <summary>Gets a value indicating whether a Hat Slime already exists in the world.</summary>
    [MemberNotNullWhen(true, "HatSlime")]
    internal static bool TheHatSlimeIsUponUs => HatSlime is not null;

    /// <summary>Gets the <see cref="GreenSlime"/> instance.</summary>
    internal GreenSlime Slime { get; }

    /// <summary>Gets the <see cref="Farmer"/> who cast <see cref="PiperConcerto"/>.</summary>
    internal Farmer Piper { get; }

    /// <summary>Gets the alpha value for drawing the <seealso cref="Slime"/>.</summary>
    internal float Alpha => this._fadeCounter >= 0
        ? (float)((-1d / (1d + Math.Exp((12d * this._fadeCounter / FADE_DURATION) - 6d))) + 1d)
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

    /// <summary>Gets a value indicating the source of the piping.</summary>
    internal PipingSource Source { get; private set; }

    /// <summary>Gets the <see cref="Color"/> value of the underlying <see cref="GreenSlime"/> instance.</summary>
    internal Color Color => this.Slime.color.Value;

    /// <summary>Gets a value indicating whether the Piped Slime's <seealso cref="Inventory"/> contains any items.</summary>
    internal bool IsCarryingItems => this.HasHat && this.Inventory.WhereNotNull().Any();

    /// <summary>Gets a value indicating whether the Piped Slime's <seealso cref="Inventory"/> has any empty slots.</summary>
    internal bool HasEmptyInventorySlots
    {
        get
        {
            if (!this.HasHat)
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

    /// <summary>Gets a value indicating whether the <seealso cref="Slime"/> is wearing a <seealso cref="Hat"/>.</summary>
    [MemberNotNullWhen(true, nameof(Hat))]
    internal bool HasHat => this.Hat is not null;

    /// <summary>Gets or sets the currently worn <seealso cref="Hat"/>>.</summary>
    private Hat? Hat
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
            if (value is not null)
            {
                HatSlime = this;
            }
        }
    }

    /// <summary>Unregisters from Pathfinders.</summary>
    public void Dispose()
    {
        Pathfinder?.Unregister(this.Slime);
        PathfinderAsync?.Unregister(this.Slime);
    }

    /// <summary>Checks for actions on the instance.</summary>
    /// <param name="who">The <see cref="Farmer"/> who is checking.</param>
    /// <param name="hat">A <see cref="Hat"/>.</param>
    internal void CheckAction(Farmer who, Hat? hat = null)
    {
        if (!ReferenceEquals(who, this.Piper))
        {
            return;
        }

        if (hat is not null)
        {
            if (this.HasHat && who.addItemToInventoryBool(this.Hat))
            {
                var slime = this.Slime;
                this.Hat = null;
                foreach (var item in this.Inventory)
                {
                    if (item is not null)
                    {
                        Game1.createItemDebris(item, slime.Position, slime.FacingDirection);
                    }
                }

                this.Inventory.Clear();

                var currentLocation = slime.currentLocation;
                var inDangerZone = currentLocation.IsEnemyArea() ||
                                     currentLocation.Name.ContainsAnyOf("Mine", "SkullCave");
                if (!inDangerZone && currentLocation.IsOutdoors)
                {
                    slime.Set_Piped(null, PipingSource.None);
                }
            }
            else if (!TheHatSlimeIsUponUs)
            {
                who.Items[who.CurrentToolIndex] = null;
                this.Hat = hat;
                this.Source = PipingSource.Hat;
                Game1.playSound("dirtyHit");
            }

            return;
        }

        if (!this.HasHat)
        {
            return;
        }

        if (this.IsCarryingItems)
        {
            this.OpenInventory();
        }
    }

    internal void Draw(SpriteBatch b, bool avoidingMate, bool pursuingMate, Vector2 facePosition)
    {
        var slime = this.Slime;
        if (slime.IsInvisible || !Utility.isOnScreen(slime.Position, 128))
        {
            return;
        }

        var boundsHeight = slime.GetBoundingBox().Height;
        var standingY = slime.StandingPixel.Y;
        for (var i = 0; i <= slime.stackedSlimes.Value; i++)
        {
            var topSlime = i == slime.stackedSlimes.Value;
            var stackAdjustment = Vector2.Zero;
            if (slime.stackedSlimes.Value > 0)
            {
                stackAdjustment =
                    new Vector2(
                        (float)Math.Sin(slime.randomStackOffset +
                                        (Game1.currentGameTime.TotalGameTime.TotalSeconds * Math.PI * 2.0) + (i * 30)) *
                        8f,
                        -30 * i);
            }

            b.Draw(
                slime.Sprite.Texture,
                slime.getLocalPosition(Game1.viewport) +
                new Vector2(32f, (boundsHeight / 2) + slime.yOffset) + stackAdjustment,
                slime.Sprite.SourceRect,
                (slime.prismatic.Value
                    ? Utility.GetPrismaticColor(348 + slime.specialNumber.Value, 5f)
                    : slime.color.Value) * this.Alpha,
                0f,
                new Vector2(8f, 16f),
                4f * Math.Max(0.2f, slime.Scale - (0.4f * (slime.ageUntilFullGrown.Value / 120000f))),
                SpriteEffects.None,
                Math.Max(0f, slime.drawOnTop ? 0.991f : (standingY + (i * 2)) / 10000f));
            b.Draw(
                Game1.shadowTexture,
                slime.getLocalPosition(Game1.viewport) + new Vector2(
                    32f,
                    (boundsHeight / 2 * 7 / 4f) + slime.yOffset + (8f * slime.Scale) -
                    (slime.ageUntilFullGrown.Value > 0 ? 8 : 0)) + stackAdjustment,
                Game1.shadowTexture.Bounds,
                Color.White * this.Alpha,
                0f,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                3f + slime.Scale - (slime.ageUntilFullGrown.Value / 120000f) -
                ((slime.Sprite.currentFrame % 4) % 3 != 0 || i != 0 ? 1f : 0f) + (slime.yOffset / 30f),
                SpriteEffects.None,
                (standingY - 1 + (i * 2)) / 10000f);
            if (slime.ageUntilFullGrown.Value <= 0)
            {
                if (topSlime && (slime.cute.Value || slime.hasSpecialItem.Value) && !this.HasHat)
                {
                    var xDongleSource = slime.isMoving() || slime.wagTimer > 0
                        ? (16 * Math.Min(
                            7,
                            Math.Abs((slime.wagTimer > 0
                                ? 992 - slime.wagTimer
                                : Game1.currentGameTime.TotalGameTime.Milliseconds % 992) - 496) / 62)) % 64
                        : 48;
                    var yDongleSource = slime.isMoving() || slime.wagTimer > 0
                        ? 24 * Math.Min(
                            1,
                            Math.Max(
                                1,
                                Math.Abs((slime.wagTimer > 0
                                    ? 992 - slime.wagTimer
                                    : Game1.currentGameTime.TotalGameTime.Milliseconds % 992) - 496) / 62) / 4)
                        : 24;
                    if (slime.hasSpecialItem.Value)
                    {
                        yDongleSource += 48;
                    }

                    b.Draw(
                        slime.Sprite.Texture,
                        slime.getLocalPosition(Game1.viewport) + stackAdjustment + (new Vector2(
                                32f,
                                boundsHeight - 16 + (slime.readyToJump <= 0
                                    ? 4 * (-2 + Math.Abs((slime.Sprite.currentFrame % 4) - 2))
                                    : 4 + (4 * ((slime.Sprite.currentFrame % 4) % 3))) + slime.yOffset) *
                            slime.Scale),
                        new Rectangle(xDongleSource, 168 + yDongleSource, 16, 24),
                        (slime.hasSpecialItem.Value ? Color.White : slime.color.Value) * this.Alpha,
                        0f,
                        new Vector2(8f, 16f),
                        4f * Math.Max(0.2f, slime.Scale - (0.4f * (slime.ageUntilFullGrown.Value / 120000f))),
                        slime.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        Math.Max(0f, slime.drawOnTop ? 0.991f : (standingY / 10000f) + 0.0001f));
                }

                b.Draw(
                    slime.Sprite.Texture,
                    slime.getLocalPosition(Game1.viewport) + stackAdjustment + ((new Vector2(
                                32f,
                                (boundsHeight / 2) + (slime.readyToJump <= 0
                                    ? 4 * (-2 + Math.Abs((slime.Sprite.currentFrame % 4) - 2))
                                    : 4 - (4 * ((slime.Sprite.currentFrame % 4) % 3))) + slime.yOffset) +
                            facePosition) *
                        Math.Max(0.2f, slime.Scale - (0.4f * (slime.ageUntilFullGrown.Value / 120000f)))),
                    new Rectangle(
                        32 + (slime.readyToJump > 0 || slime.focusedOnFarmers ? 16 : 0),
                        120 + (slime.readyToJump < 0 &&
                               (slime.focusedOnFarmers || slime.invincibleCountdown > 0)
                            ? 24
                            : 0),
                        16,
                        24),
                    (Color.White * (slime.FacingDirection == 0 ? 0.5f : 1f)) * this.Alpha,
                    0f,
                    new Vector2(8f, 16f),
                    4f * Math.Max(0.2f, slime.Scale - (0.4f * (slime.ageUntilFullGrown.Value / 120000f))),
                    SpriteEffects.None,
                    Math.Max(0f, slime.drawOnTop ? 0.991f : ((standingY + (i * 2)) / 10000f) + 0.0001f));
            }

            if (slime.isGlowing)
            {
                b.Draw(
                    slime.Sprite.Texture,
                    slime.getLocalPosition(Game1.viewport) + stackAdjustment +
                    new Vector2(32f, (boundsHeight / 2) + slime.yOffset),
                    slime.Sprite.SourceRect,
                    (slime.glowingColor * slime.glowingTransparency) * this.Alpha,
                    0f,
                    new Vector2(8f, 16f),
                    4f * Math.Max(0.2f, slime.Scale),
                    SpriteEffects.None,
                    Math.Max(0f, slime.drawOnTop ? 0.99f : (standingY / 10000f) + 0.001f));
            }
        }

        if (pursuingMate)
        {
            b.Draw(
                slime.Sprite.Texture,
                slime.getLocalPosition(Game1.viewport) + new Vector2(32f, -32 + slime.yOffset),
                new Rectangle(16, 120, 8, 8),
                Color.White * this.Alpha,
                0f,
                new Vector2(3f, 3f),
                4f,
                SpriteEffects.None,
                Math.Max(0f, slime.drawOnTop ? 0.991f : slime.StandingPixel.Y / 10000f));
        }
        else if (avoidingMate)
        {
            b.Draw(
                slime.Sprite.Texture,
                slime.getLocalPosition(Game1.viewport) + new Vector2(32f, -32 + slime.yOffset),
                new Rectangle(24, 120, 8, 8),
                Color.White * this.Alpha,
                0f,
                new Vector2(4f, 4f),
                4f,
                SpriteEffects.None,
                Math.Max(0f, slime.drawOnTop ? 0.991f : slime.StandingPixel.Y / 10000f));
        }

        if (this.HasHat)
        {
            this.DrawHat(this.Hat, b);
        }
    }

    /// <summary>Updates the instance state and counts down necessary timers.</summary>
    /// <param name="time">The current <see cref="GameTime"/>.</param>
    internal void Update(GameTime time)
    {
        if (this._fadeCounter < 0)
        {
            return;
        }

        this._fadeCounter += this._fadeDelta;
        if (this._fadeCounter is < FADE_DURATION and >= 0)
        {
            return;
        }

        this._fadeCounter = -1;
        this._fadeDelta = 0;
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

    /// <summary>Initiates fade in.</summary>
    internal void FadeIn()
    {
        this._fadeCounter = 0;
        this._fadeDelta = 1;
        this.Slime.currentLocation.characters.Add(this.Slime);
        this.WarpToPiper();
    }

    /// <summary>Initiates fade out.</summary>
    internal void FadeOut()
    {
        this._fadeCounter = FADE_DURATION;
        this._fadeDelta = -1;
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
        if (this.IsCarryingItems)
        {
            this.DropItems();
        }

        slime.Health = 0;
        slime.deathAnimation();
        slime.Set_Piped(null, PipingSource.None);
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

    /// <summary>Draws the <seealso cref="Hat"/>.</summary>
    /// <param name="hat">The <see cref="Hat"/>.</param>
    /// <param name="b">The <see cref="SpriteBatch"/>.</param>
    private void DrawHat(Hat hat, SpriteBatch b)
    {
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
        hat.draw(
            b,
            Utility.snapDrawPosition(slime.getLocalPosition(Game1.viewport) - new Vector2(4f, yOffset)),
            1.3333334f,
            1f,
            (slime.StandingPixel.Y / 10000f) + 1E-07f,
            slime.FacingDirection);
    }
}
