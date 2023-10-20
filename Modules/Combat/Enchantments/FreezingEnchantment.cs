namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Extensions;
using DaLion.Overhaul.Modules.Combat.StatusEffects;
using DaLion.Overhaul.Modules.Core;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Moving and attacking generates Energize stacks, up to 100. At maximum stacks, the next attack causes an electric discharge,
///     dealing heavy damage in a large area.
/// </summary>
/// <remarks>6 charges per hit + 1 charge per 6 tiles traveled.</remarks>
[XmlType("Mods_DaLion_FreezingEnchantment")]
public sealed class FreezingEnchantment : BaseSlingshotEnchantment
{
    /// <summary>Finalizes an instance of the <see cref="FreezingEnchantment"/> class.</summary>
    ~FreezingEnchantment()
    {
        EventManager.Disable<EnergizedUpdateTickedEvent>();
    }

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Freezing_Name();
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        monster.Chill(2000, 0.2f, 0.5f);
        SoundEffectPlayer.ChillingShot.Play();
    }
}
