namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley.Monsters;
using System.Xml.Serialization;

#endregion using directives

/// <summary>Slain monsters award gold equivalent to 10% of their max health.</summary>
[XmlType("Mods_DaLion_TributeEnchantment")]
public class TributeEnchantment : BaseWeaponEnchantment
{
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        who.Money += (int)(m.MaxHealth * 0.1f);
    }

    public override string GetName() => ModEntry.i18n.Get("enchantments.tribute");
}