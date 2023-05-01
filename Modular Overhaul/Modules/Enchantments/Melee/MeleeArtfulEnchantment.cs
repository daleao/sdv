namespace DaLion.Overhaul.Modules.Enchantments.Melee;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Improves weapon special moves.</summary>
[XmlType("Mods_DaLion_MeleeArtfulEnchantment")]
public sealed class MeleeArtfulEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return "Artful";
    }
}
