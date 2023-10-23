namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks that would leave an enemy below 10% max health immediately execute the enemy, converting the remaining health into gold.
///     Consecutive kills without taking damage increase the threshold by 1%.
/// </summary>
[XmlType("Mods_DaLion_MammoniteEnchantment")]
public sealed class MammoniteEnchantment : BaseWeaponEnchantment
{
    internal float Threshold { get; set; } = 0.1f;

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Mammonite_Name();
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        if (monster.Health >= monster.MaxHealth * this.Threshold || monster.Health > 1000)
        {
            return;
        }

        var chance = 1d - (monster.Health / 1000d);
        if (Game1.random.NextDouble() > chance)
        {
            return;
        }

        who.Money += monster.Health;
        monster.Health = 0;
    }

    /// <inheritdoc />
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        this.Threshold += 0.01f;
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        if (who.IsLocalPlayer)
        {
            this.Threshold = 0;
        }
    }
}
