namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Enables auto-firing at lower firing speed..</summary>
[XmlType("Mods_DaLion_GatlingEnchantment")]
public class GatlingEnchantment : BaseSlingshotEnchantment
{
    public override string GetName() => ModEntry.i18n.Get("enchantments.gatling");
}