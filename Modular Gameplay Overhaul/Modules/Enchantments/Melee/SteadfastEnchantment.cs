// ReSharper disable EqualExpressionComparison
namespace DaLion.Overhaul.Modules.Enchantments.Melee;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Converts critical strike chance and power into raw damage.</summary>
[XmlType("Mods_DaLion_SteadfastEnchantment")]
public sealed class SteadfastEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.steadfast.name");
    }
}
