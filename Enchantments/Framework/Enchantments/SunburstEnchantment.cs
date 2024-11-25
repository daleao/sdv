namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Projectiles;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>
///     Attacks fire a short-range beam of solar energy. Prevents mummies from resurrecting.
/// </summary>
[XmlType("Mods_DaLion_SunburstEnchantment")]
public sealed class SunburstEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Sunburst_Name();
    }

    /// <inheritdoc />
    public override void OnCalculateDamage(Monster monster, GameLocation location, Farmer who, bool fromBomb, ref int amount)
    {
        base.OnCalculateDamage(monster, location, who, fromBomb, ref amount);
        if (monster is Ghost or Skeleton or Mummy or ShadowBrute or ShadowShaman or ShadowGirl or ShadowGuy or Shooter)
        {
            amount = (int)(amount * 1.5f);
        }
    }

    /// <inheritdoc />
    protected override void _OnSwing(MeleeWeapon weapon, Farmer farmer)
    {
        base._OnSwing(weapon, farmer);
        var origin = farmer.getStandingPosition() - new Vector2(32f, 32f);
        var velocity = default(Vector2);
        switch (farmer.facingDirection.Value)
        {
            case 0:
                velocity.Y = -1f;
                break;
            case 1:
                velocity.X = 1f;
                break;
            case 3:
                velocity.X = -1f;
                break;
            case 2:
                velocity.Y = 1f;
                break;
        }

        velocity *= 10f;
        farmer.currentLocation.projectiles.Add(new SunburstProjectile(farmer, weapon, origin, velocity, 32f));
    }
}
