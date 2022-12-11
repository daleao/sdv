namespace DaLion.Ligo.Modules.Arsenal.Enchantments;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Enables auto-firing at lower firing speed.</summary>
[XmlType("Mods_DaLion_GatlingEnchantment")]
public sealed class GatlingEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return i18n.Get("enchantments.gatling");
    }
}
