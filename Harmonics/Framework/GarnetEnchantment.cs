namespace DaLion.Harmonics.Framework;

#region using directives

using System.Xml.Serialization;
using DaLion.Harmonics.Framework.VirtualProperties;
using StardewValley;
using StardewValley.Enchantments;
using StardewValley.Tools;

#endregion using directives

/// <summary>The Garnet gemstone forge.</summary>
[XmlType("Mods_DaLion_GarnetEnchantment")]
public sealed class GarnetEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override bool ShouldBeDisplayed()
    {
        return false;
    }

    /// <inheritdoc />
    public override bool IsForge()
    {
        return true;
    }

    /// <inheritdoc />
    protected override void _ApplyTo(Item item)
    {
        base._ApplyTo(item);
        if (item is not MeleeWeapon weapon)
        {
            return;
        }

        var value = 0.05f * this.GetLevel();
        weapon.Get_CooldownReduction().Value += value;
        Data.Increment(weapon, DataKeys.CooldownReduction, value);
    }

    /// <inheritdoc />
    protected override void _UnapplyTo(Item item)
    {
        base._UnapplyTo(item);
        if (item is not MeleeWeapon weapon)
        {
            return;
        }

        var value = 0.05f * this.GetLevel();
        weapon.Get_CooldownReduction().Value -= value;
        Data.Increment(weapon, DataKeys.CooldownReduction, -value);
    }
}
