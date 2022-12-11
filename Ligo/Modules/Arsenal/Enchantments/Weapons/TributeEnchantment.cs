namespace DaLion.Ligo.Modules.Arsenal.Enchantments;

#region using directives

using System.Xml.Serialization;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold.
///     For undead enemies, the threshold is 20%.
/// </summary>
[XmlType("Mods_DaLion_TributeEnchantment")]
public class TributeEnchantment : BaseWeaponEnchantment
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public override string GetName()
    {
        return i18n.Get("enchantments.tribute");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        var tribute = (int)((monster.MaxHealth * 0.1f) - monster.Health);
        if (tribute <= 0)
        {
            return;
        }

        monster.Health = 0;
        who.Money += tribute;
    }
}
