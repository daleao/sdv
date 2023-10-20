namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Integrations;
using Microsoft.Xna.Framework;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>The base class for <see cref="Slingshot"/> weapon enchantments.</summary>
[XmlType("Mods_DaLion_BaseSlingshotEnchantment")]
public class BaseSlingshotEnchantment : BaseEnchantment
{
    /// <inheritdoc />
    public override bool CanApplyTo(Item item)
    {
        return item is Slingshot slingshot &&
               ArcheryIntegration.Instance?.ModApi?.GetWeaponData(Manifest, slingshot) is null;
    }

    /// <summary>Raised when the <paramref name="slingshot"/> fires a <see cref="BasicProjectile"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="projectile">The fired <see cref="BasicProjectile"/>.</param>
    /// <param name="overcharge">The amount of overcharge applied by <see cref="Professions.Profession.Desperado"/>.</param>
    /// <param name="startingPosition">The projectile's starting position (also its current position).</param>
    /// <param name="xVelocity">The horizontal component of projectile's velocity.</param>
    /// <param name="yVelocity">The vertical component of projectile's velocity.</param>
    /// <param name="rotationVelocity">The projectile's rotational velocity.</param>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired the shot.</param>
    public void OnFire(
        Slingshot slingshot,
        BasicProjectile projectile,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        GameLocation location,
        Farmer firer)
    {
        this._OnFire(
            slingshot,
            projectile,
            overcharge,
            startingPosition,
            xVelocity,
            yVelocity,
            rotationVelocity,
            location,
            firer);
    }

    /// <inheritdoc cref="OnFire"/>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Copies vanilla style.")]
    protected virtual void _OnFire(
        Slingshot slingshot,
        BasicProjectile projectile,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        GameLocation location,
        Farmer firer)
    {
    }
}
