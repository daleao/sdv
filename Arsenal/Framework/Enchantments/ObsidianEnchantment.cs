namespace DaLion.Arsenal.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Arsenal.Framework.Integrations;
using DaLion.Core.Framework.Extensions;
using StardewValley.Enchantments;
using StardewValley.Monsters;

#endregion using directives

/// <summary>The secondary <see cref="BaseWeaponEnchantment"/> which characterizes the Lava Katana.</summary>
[XmlType("Mods_DaLion_ObsidianEnchantment")]
public sealed class ObsidianEnchantment : BaseWeaponEnchantment
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
    public override void OnCalculateDamage(Monster monster, GameLocation location, Farmer who, bool fromBomb, ref int amount)
    {
        if (fromBomb)
        {
            return;
        }

        if (CombatIntegration.Instance?.ModApi?.GetConfig().GeometricMitigationFormula == true)
        {
            amount = (int)(amount * Math.Pow(1.1, monster.resilience.Value));
        }
        else
        {
            amount += monster.resilience.Value;
        }
    }

    /// <inheritdoc />
    public override void OnDealtDamage(Monster monster, GameLocation location, Farmer who, bool fromBomb, int amount)
    {
        if (fromBomb)
        {
            return;
        }

        if (this._random.NextDouble() < 0.2)
        {
            monster.Bleed(who);
        }
    }
}
