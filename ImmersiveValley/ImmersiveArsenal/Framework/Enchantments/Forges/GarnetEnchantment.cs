namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;

#endregion using directives

[XmlType("Mods_DaLion_GarnetEnchantment")]
public class GarnetEnchantment : BaseWeaponEnchantment
{
	protected override void _ApplyTo(Item item)
    {
        base._ApplyTo(item);
    }

    protected override void _UnapplyTo(Item item)
    {
        base._UnapplyTo(item);
    }

    public override bool ShouldBeDisplayed() => false;

    public override bool IsForge() => true;
}