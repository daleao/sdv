namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Common.Extensions.Stardew;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Attacks on-hit deal 60% - 20% (based on distance) damage to other enemies near the target.</summary>
[XmlType("Mods_DaLion_CleavingEnchantment")]
public class CleavingEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return ModEntry.i18n.Get("enchantments.cleaving");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        for (var i = location.characters.Count - 1; i >= 0; --i)
        {
            var character = location.characters[i];
            if (character is not Monster { IsMonster: true, Health: > 0 } other || other.IsInvisible ||
                other.isInvincible())
            {
                continue;
            }

            var distance = other.DistanceTo(monster);
            if (distance > 3)
            {
                continue;
            }

            var damage = (int)(amount * (0.8 - (0.2 * distance)));
            var (x, y) = Utility.getAwayFromPositionTrajectory(other.GetBoundingBox(), monster.Position);
            other.takeDamage(damage, (int)x, (int)y, false, double.MaxValue, who);
        }
    }
}
