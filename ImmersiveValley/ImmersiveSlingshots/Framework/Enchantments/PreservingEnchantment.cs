namespace DaLion.Stardew.Slingshots.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Chance to not consume ammo.</summary>
[XmlType("Mods_DaLion_PreservingEnchantment")]
public class PreservingEnchantment : BaseSlingshotEnchantment
{
    public override string GetName() => ModEntry.i18n.Get("enchantments.preserving");
}