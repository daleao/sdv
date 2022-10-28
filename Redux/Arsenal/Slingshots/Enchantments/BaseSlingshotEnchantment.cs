namespace DaLion.Redux.Arsenal.Slingshots.Enchantments;

#region using directives

using System.Xml.Serialization;
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
        return item is Slingshot && ModEntry.Config.Arsenal.Slingshots.AllowEnchants;
    }

    /// <summary>Raised when the <paramref name="slingshot"/> fires a <see cref="BasicProjectile"/>.</summary>
    /// <param name="slingshot">The <see cref="Slingshot"/>.</param>
    /// <param name="projectile">The fired <see cref="BasicProjectile"/>.</param>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="firer">The <see cref="Farmer"/> who fired the shot.</param>
    public void OnFire(Slingshot slingshot, BasicProjectile projectile, GameLocation location, Farmer firer)
    {
        this._OnFire(slingshot, projectile, location, firer);
    }

    /// <inheritdoc cref="OnFire"/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "Copies vanilla style.")]
    protected virtual void _OnFire(
        Slingshot slingshot, BasicProjectile projectile, GameLocation location, Farmer firer)
    {
    }
}
