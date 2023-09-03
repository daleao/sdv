namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Chance to not consume ammo.</summary>
[XmlType("Mods_DaLion_MagnumEnchantment")]
public sealed class MagnumEnchantment : BaseSlingshotEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Magnum_Name();
    }
}
