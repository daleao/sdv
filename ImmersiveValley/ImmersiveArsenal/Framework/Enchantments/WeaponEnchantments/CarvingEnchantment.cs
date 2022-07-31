using System;

namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley.Monsters;
using System.Xml.Serialization;

#endregion using directives

/// <summary>Attacks on-hit deal 60% - 20% (based on distance) damage to other enemies near the target.</summary>
[XmlType("Mods_DaLion_CarvingEnchantment")]
public class CarvingEnchantment : BaseWeaponEnchantment
{
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        monster.resilience.Value = Math.Min(monster.resilience.Value - 2, -2);
    }

    public override string GetName() => ModEntry.i18n.Get("enchantments.carving");
}