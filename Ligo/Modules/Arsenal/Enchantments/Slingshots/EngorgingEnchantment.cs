namespace DaLion.Ligo.Modules.Arsenal.Enchantments;

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
        return ModEntry.i18n.Get("enchantments.engorging");
    }
}
