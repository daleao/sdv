namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Events.Player.Warped;
using DaLion.Overhaul.Modules.Combat.Projectiles;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes Infinity weapons.</summary>
[XmlType("Mods_DaLion_InfinityEnchantment")]
public sealed class InfinityEnchantment : BaseWeaponEnchantment
{
    private readonly Color _lightSourceColor = Color.DeepPink.Inverse();
    private readonly float _lightSourceRadius = 2.5f;
    private int? _lightSourceId;
    private LightSource? _lightSource;

    /// <inheritdoc />
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    /// <inheritdoc />
    public override bool IsForge()
    {
        return false;
    }

    /// <inheritdoc />
    public override int GetMaximumLevel()
    {
        return 1;
    }

    /// <inheritdoc />
    public override bool ShouldBeDisplayed()
    {
        return false;
    }

    /// <inheritdoc />
    public override bool CanApplyTo(Item item)
    {
        return item is MeleeWeapon weapon && weapon.GetEnchantmentLevel<GalaxySoulEnchantment>() >= 3;
    }

    internal void OnWarp(Farmer who, GameLocation oldLocation, GameLocation newLocation)
    {
        if (this._lightSourceId.HasValue)
        {
            oldLocation.removeLightSource(this._lightSourceId.Value);
        }
        else
        {
            this._lightSourceId ??= this.GetHashCode() + (int)who.UniqueMultiplayerID;
        }

        while (newLocation.sharedLights.ContainsKey(this._lightSourceId!.Value))
        {
            this._lightSourceId++;
        }

        if (this._lightSource is null)
        {
            this._lightSource = new LightSource(
                LightSource.lantern,
                new Vector2(who.Position.X + 26f, who.Position.Y + 64f),
                this._lightSourceRadius,
                this._lightSourceColor,
                this._lightSourceId.Value,
                LightSource.LightContext.None,
                who.UniqueMultiplayerID);
        }
        else
        {
            this._lightSource.Identifier = this._lightSourceId.Value;
        }

        newLocation.sharedLights[this._lightSourceId.Value] = this._lightSource;
    }

    internal void Update(uint ticks, Farmer who)
    {
        if (this._lightSource is null || !this._lightSourceId.HasValue)
        {
            return;
        }

        var offset = Vector2.Zero;
        if (who.shouldShadowBeOffset)
        {
            offset += who.drawOffset.Value;
        }

        this._lightSource.position.Value = new Vector2(who.Position.X + 26f, who.Position.Y + 16f) + offset;

        if (ticks % 10 == 0)
        {
            this._lightSource.radius.Value = this._lightSourceRadius + (float)Game1.random.NextGaussian(0.5, 0.025);
        }
    }

    /// <inheritdoc />
    protected override void _OnEquip(Farmer who)
    {
        base._OnEquip(who);
        this._lightSourceId ??= this.GetHashCode() + (int)who.UniqueMultiplayerID;
        var location = who.currentLocation;
        while (location.sharedLights.ContainsKey(this._lightSourceId!.Value))
        {
            this._lightSourceId++;
        }

        if (this._lightSource is null)
        {
            this._lightSource = new LightSource(
                LightSource.lantern,
                new Vector2(who.Position.X + 26f, who.Position.Y + 64f),
                this._lightSourceRadius,
                this._lightSourceColor,
                this._lightSourceId.Value,
                LightSource.LightContext.None,
                who.UniqueMultiplayerID);
        }
        else
        {
            this._lightSource.Identifier = this._lightSourceId.Value;
        }

        location.sharedLights[this._lightSourceId.Value] = this._lightSource;
        EventManager.Enable(typeof(BaseEnchantmentUpdateTickedEvent), typeof(BaseEnchantmentWarpedEvent));
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        if (this._lightSource is null || !this._lightSourceId.HasValue)
        {
            return;
        }

        var location = who.currentLocation;
        location.removeLightSource(this._lightSourceId.Value);
        this._lightSource = null;
        EventManager.Disable(typeof(BaseEnchantmentUpdateTickedEvent), typeof(BaseEnchantmentWarpedEvent));
    }

    /// <inheritdoc />
    protected override void _OnSwing(MeleeWeapon weapon, Farmer farmer)
    {
        base._OnSwing(weapon, farmer);
        if (farmer.health < farmer.maxHealth)
        {
            return;
        }

        var facingDirection = (FacingDirection)farmer.FacingDirection;
        var facingVector = facingDirection.ToVector();
        var startingPosition = farmer.getStandingPosition() + (facingVector * 64f) - new Vector2(32f, 32f);
        var velocity = facingVector * 10f;
        var rotation = (float)Math.PI / 180f * 32f;
        farmer.currentLocation.projectiles.Add(new InfinityProjectile(
            weapon,
            farmer,
            startingPosition,
            velocity.X,
            velocity.Y,
            rotation));
    }
}
