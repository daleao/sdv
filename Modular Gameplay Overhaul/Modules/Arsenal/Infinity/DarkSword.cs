namespace DaLion.Overhaul.Modules.Arsenal.Infinity;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
using StardewValley.Tools;

#endregion using directives

/// <summary>The Dark Sword.</summary>
/// <remarks>Unused.</remarks>
[XmlType("Mods_DaLion_DarkSword")]
public sealed class DarkSword : MeleeWeapon
{
    /// <summary>Initializes a new instance of the <see cref="DarkSword"/> class.</summary>
    public DarkSword()
    : base(ItemIDs.DarkSword)
    {
        this.AddEnchantment(new CursedEnchantment());
        this.specialItem = true;
    }
}
