namespace DaLion.Arsenal.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley.Enchantments;
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
        var startingPosition = farmer.StandingPixel.ToVector2() + (facingVector * 64f) - new Vector2(32f, 32f);
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
