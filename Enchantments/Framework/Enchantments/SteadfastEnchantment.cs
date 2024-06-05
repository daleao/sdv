// ReSharper disable EqualExpressionComparison
namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;

#endregion using directives

/// <summary>Cannot crit. Converts critical strike chance and power into raw damage.</summary>
[XmlType("Mods_DaLion_SteadfastEnchantment")]
public sealed class SteadfastEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Steadfast_Name();
    }
}
