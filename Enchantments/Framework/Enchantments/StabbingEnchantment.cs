namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using StardewValley.Tools;

#endregion using directives

/// <summary>Replaces the defensive parry special move with a stabbing lunge attack.</summary>
[XmlType("Mods_DaLion_StabbingEnchantment")]
public sealed class StabbingEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override bool CanApplyTo(Item item)
    {
        return item is MeleeWeapon weapon && weapon.type.Value == MeleeWeapon.defenseSword && !weapon.isScythe();
    }

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Stabbing_Name();
    }
}
