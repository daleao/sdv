namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley.Monsters;
using System;
using System.Xml.Serialization;
using VirtualProperties;

#endregion using directives

/// <summary>Slain monsters award gold equivalent to 10% of their max health.</summary>
[XmlType("Mods_DaLion_TributeEnchantment")]
public class TributeEnchantment : BaseWeaponEnchantment
{
    private Random r = new(Guid.NewGuid().GetHashCode());

    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        who.Money += (int)(m.MaxHealth * 0.1f);

        var chance = 0.1 + (double)m.get_Overkill() / m.MaxHealth;
        if (r.NextDouble() > chance) return;
        
        var (x, y) = m.GetBoundingBox().Center;
        location.monsterDrop(m, x, y, who);
    }

    public override string GetName() => ModEntry.i18n.Get("enchantments.tribute");
}