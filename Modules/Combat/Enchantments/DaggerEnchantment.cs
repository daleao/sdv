namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Extensions;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>A secondary <see cref="BaseWeaponEnchantment"/> shared by all daggers.</summary>
[XmlType("Mods_DaLion_DaggerEnchantment")]
public class DaggerEnchantment : BaseWeaponEnchantment
{
    private readonly Random _random = new(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public override bool IsSecondaryEnchantment()
    {
        return true;
    }

    /// <inheritdoc />
    public override bool IsForge()
    {
        return false;
    }

    /// <inheritdoc />
    public override int GetMaximumLevel()
    {
        return 1;
    }

    /// <inheritdoc />
    public override bool ShouldBeDisplayed()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        base._OnDealDamage(monster, location, who, ref amount);
        if (CombatModule.ShouldEnable && this._random.NextDouble() < 0.2)
        {
            monster.Bleed(who);
        }
    }
}
