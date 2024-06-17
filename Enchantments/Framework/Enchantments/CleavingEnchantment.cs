namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Attacks on-hit spread 60% - 20% (based on distance) of the damage to other enemies within 3 tiles.</summary>
[XmlType("Mods_DaLion_CleavingEnchantment")]
public sealed class CleavingEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Cleaving_Name();
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        base._OnDealDamage(monster, location, who, ref amount);
        for (var i = location.characters.Count - 1; i >= 0; i--)
        {
            var character = location.characters[i];
            if (character == monster || character is not Monster { IsMonster: true, Health: > 0 } other || other.IsInvisible ||
                other.isInvincible())
            {
                continue;
            }

            var distance = other.SquaredTileDistance(who.Tile);
            if (distance > 9)
            {
                continue;
            }

            var damage = (int)(amount * (0.8 - (0.2 * Math.Sqrt(distance))));
            if (damage <= 0)
            {
                continue;
            }

            var (x, y) = Utility.getAwayFromPositionTrajectory(other.GetBoundingBox(), monster.Position);
            other.takeDamage(damage, (int)x, (int)y, false, double.MaxValue, who);
            location.debris.Add(new Debris(
                damage,
                new Vector2(other.StandingPixel.X, other.StandingPixel.Y),
                new Color(255, 130, 0),
                1f,
                who));
            if (other.Health <= 0)
            {
                other.Die(who);
            }
        }
    }
}
