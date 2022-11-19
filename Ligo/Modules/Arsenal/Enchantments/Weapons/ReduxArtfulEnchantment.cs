namespace DaLion.Ligo.Modules.Arsenal.Enchantments;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Improves weapon special moves.</summary>
[XmlType("Mods_DaLion_ArtfulEnchantment")]
public class ReduxArtfulEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return "Artful";
    }
}
