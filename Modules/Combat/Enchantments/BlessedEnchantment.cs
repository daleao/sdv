namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Events.Player.Warped;
using DaLion.Overhaul.Modules.Combat.Projectiles;
using DaLion.Shared.Enums;
using Microsoft.Xna.Framework;
using Shared.Extensions.Xna;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Holy Blade.</summary>
[XmlType("Mods_DaLion_BlessedEnchantment")]
public class BlessedEnchantment : BaseWeaponEnchantment
{
    private readonly Color _lightSourceColor = Color.Yellow.Inverse();
    private readonly float _lightSourceRadius = 2f;
    private int? _lightSourceId;

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

        newLocation.sharedLights[this._lightSourceId.Value] = new LightSource(
            LightSource.lantern,
            new Vector2(who.Position.X + 26f, who.Position.Y + 64f),
            this._lightSourceRadius,
            this._lightSourceColor,
            this._lightSourceId.Value,
            LightSource.LightContext.None,
            who.UniqueMultiplayerID);
    }

    internal void Update(Farmer who)
    {
        if (!this._lightSourceId.HasValue)
        {
            return;
        }

        var offset = Vector2.Zero;
        if (who.shouldShadowBeOffset)
        {
            offset += who.drawOffset.Value;
        }

        who.currentLocation.repositionLightSource(
            this._lightSourceId.Value,
            new Vector2(who.Position.X + 26f, who.Position.Y + 16) + offset);
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (monster is Ghost or Skeleton or Mummy or ShadowBrute or ShadowShaman or ShadowGirl or ShadowGuy or Shooter)
        {
            amount = (int)(amount * 1.2f);
        }
    }

    /// <inheritdoc />
    protected override void _OnEquip(Farmer who)
    {
        base._OnEquip(who);
        if (!who.mailReceived.Contains("gotHolyBlade"))
        {
            who.mailReceived.Add("gotHolyBlade");
        }

        this._lightSourceId ??= this.GetHashCode() + (int)who.UniqueMultiplayerID;
        var location = who.currentLocation;
        while (location.sharedLights.ContainsKey(this._lightSourceId!.Value))
        {
            this._lightSourceId++;
        }

        location.sharedLights[this._lightSourceId.Value] = new LightSource(
            LightSource.lantern,
            new Vector2(who.Position.X + 26f, who.Position.Y + 64f),
            this._lightSourceRadius,
            this._lightSourceColor,
            this._lightSourceId.Value,
            LightSource.LightContext.None,
            who.UniqueMultiplayerID);
        EventManager.Enable(typeof(WeaponEnchantmentUpdateTickedEvent), typeof(WeaponEnchantmentWarpedEvent));
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        if (!this._lightSourceId.HasValue)
        {
            return;
        }

        var location = who.currentLocation;
        location.removeLightSource(this._lightSourceId.Value);
        EventManager.Disable(typeof(WeaponEnchantmentUpdateTickedEvent), typeof(WeaponEnchantmentWarpedEvent));
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
        farmer.currentLocation.projectiles.Add(new LightBeamProjectile(
            weapon,
            farmer,
            startingPosition,
            velocity.X,
            velocity.Y,
            rotation));
    }
}
