namespace DaLion.Overhaul.Modules.Enchantments.Ranged;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Chance to not consume ammo.</summary>
[XmlType("Mods_DaLion_EngorgingEnchantment")]
public sealed class EngorgingEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.engorging.name");
    }
}
