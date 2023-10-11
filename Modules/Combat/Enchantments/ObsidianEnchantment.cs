namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Extensions;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Lava Katana.</summary>
[XmlType("Mods_DaLion_ObsidianEnchantment")]
public class ObsidianEnchantment : BaseWeaponEnchantment
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

    /// <summary>Invoked once damage to monster has been calculated, but before it is applied.</summary>
    /// <param name="monster">The <see cref="Monster"/> being hit.</param>
    /// <param name="location">The <see cref="GameLocation"/>.</param>
    /// <param name="who">The <see cref="Farmer"/> inflicting damage.</param>
    /// <param name="amount">The calculated damage amount.</param>
    public new void OnCalculateDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (CombatModule.Config.NewResistanceFormula)
        {
            amount *= 1 + (monster.resilience.Value / 10);
        }
        else
        {
            amount += monster.resilience.Value;
        }
    }
}
