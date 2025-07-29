namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Shared.Constants;
using StardewValley.Tools;

#endregion using directives

/// <summary>Replaces the defensive parry special move with a stabbing lunge attack.</summary>
[XmlType("Mods_DaLion_SharpEnchantment")]
public sealed class SharpEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override bool CanApplyTo(Item item)
    {
        return (item is MeleeWeapon weapon && weapon.isScythe() && weapon.QualifiedItemId == QIDs.IridiumScythe) ||
               item is Pickaxe { UpgradeLevel: >= 4 };
    }

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Stabbing_Name();
    }

    /// <inheritdoc />
    protected override void _ApplyTo(Item item)
    {
        base._ApplyTo(item);
        if (item is MeleeWeapon weapon && MeleeWeapon.TryGetData(QIDs.GalaxySword, out var galaxySwordData))
        {
            weapon.minDamage.Value = galaxySwordData.MinDamage;
            weapon.maxDamage.Value = galaxySwordData.MaxDamage;
        }
    }

    /// <inheritdoc />
    protected override void _UnapplyTo(Item item)
    {
        base._UnapplyTo(item);
        if (item is MeleeWeapon weapon && MeleeWeapon.TryGetData(QIDs.IridiumScythe, out var iridiumScythe))
        {
            weapon.minDamage.Value = iridiumScythe.MinDamage;
            weapon.maxDamage.Value = iridiumScythe.MaxDamage;
        }
    }
}
