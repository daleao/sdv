namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using System;
using System.Xml.Serialization;
using DaLion.Stardew.Arsenal.Framework.VirtualProperties;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Slain monsters award gold equivalent to 10% of their max health.</summary>
[XmlType("Mods_DaLion_TributeEnchantment")]
public class TributeEnchantment : BaseWeaponEnchantment
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public override string GetName()
    {
        return ModEntry.i18n.Get("enchantments.tribute");
    }

    /// <inheritdoc />
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        who.Money += (int)(m.MaxHealth * 0.1f);

        var chance = 0.1 + ((double)m.Get_Overkill() / m.MaxHealth);
        if (this._random.NextDouble() > chance)
        {
            return;
        }

        var (x, y) = m.GetBoundingBox().Center;
        location.monsterDrop(m, x, y, who);
    }
}
