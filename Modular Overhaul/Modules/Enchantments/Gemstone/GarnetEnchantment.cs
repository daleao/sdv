namespace DaLion.Overhaul.Modules.Enchantments.Gemstone;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Overhaul.Modules.Weapons.VirtualProperties;
using StardewValley;
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
    }

    /// <inheritdoc />
    protected override void _UnapplyTo(Item item)
    {
        base._UnapplyTo(item);
        switch (item)
        {
            case MeleeWeapon weapon when WeaponsModule.ShouldEnable:
                weapon.Invalidate();
                break;
            case Slingshot slingshot when SlingshotsModule.ShouldEnable:
                slingshot.Invalidate();
                break;
        }
    }
}
