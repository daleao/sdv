namespace DaLion.Ligo.Modules.Arsenal.Slingshots.Enchantments;

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
        return ModEntry.i18n.Get("enchantments.gatling");
    }
}
