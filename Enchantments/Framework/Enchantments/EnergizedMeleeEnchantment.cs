namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using Core.Framework.Extensions;
using DaLion.Enchantments.Framework.Buffs;
using DaLion.Enchantments.Framework.Events;
using DaLion.Shared.Extensions.Xna;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>
///     Moving and attacking generates energy, up to 100 stacks. At maximum stacks, the next attack causes a powerful electric discharge which
///     deals heavy damage in a large area.
/// </summary>
/// <remarks>4 charges per hit + 1 charge per 3 tiles traveled.</remarks>
[XmlType("Mods_DaLion_EnergizedMeleeEnchantment")]
public sealed class EnergizedMeleeEnchantment : BaseWeaponEnchantment
{
    /// <summary>The amount of energy stacks when fully charged.</summary>
    public const int MaxEnergy = 100;

    private uint _previousStepsTaken;
    private int _energy = -1;
    private bool _doingLightningStrike;
    private bool _didCountThisSwipe;

    /// <summary>Finalizes an instance of the <see cref="EnergizedMeleeEnchantment"/> class.</summary>
    ~EnergizedMeleeEnchantment()
    {
        EventManager.Disable<EnergizedUpdateTickedEvent>();
    }

    /// <summary>Gets or sets the current number of energy stacks.</summary>
    public int Energy
    {
        get => this._energy;
        set
        {
            this._energy = Math.Min(value, MaxEnergy);
        }
    }

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Energized_Name();
    }

    /// <summary>Updates the instance state.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started, including the current tick.</param>
    public void Update(uint ticks)
    {
        var player = Game1.player;
        if (!player.UsingTool)
        {
            this._didCountThisSwipe = false;
        }

        if (ticks % 60 == 0)
        {
            var gained = (Game1.stats.StepsTaken - this._previousStepsTaken) / 2;
            if (gained > 0 && player.Position != player.lastPosition)
            {
                this.Energy += (int)gained;
            }

            this._previousStepsTaken = Game1.stats.StepsTaken;
        }

        if (this.Energy <= 0)
        {
            return;
        }

        player.applyBuff(new EnergizedBuff(() => this.Energy));
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (this._doingLightningStrike)
        {
            return;
        }

        if (this.Energy >= MaxEnergy)
        {
            this.Energy = 0;
            this._doingLightningStrike = true;
            location.DoLightningBarrage(monster.Tile, 6, who);
            this._doingLightningStrike = false;
        }
        else if (!this._didCountThisSwipe)
        {
            this.Energy += 4;
            this._didCountThisSwipe = true;
        }
    }

    /// <inheritdoc />
    protected override void _OnEquip(Farmer who)
    {
        base._OnEquip(who);
        if (!who.IsLocalPlayer)
        {
            return;
        }

        this._previousStepsTaken = Game1.stats.StepsTaken;
        EventManager.Enable<EnergizedUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        if (!who.IsLocalPlayer)
        {
            return;
        }

        this._previousStepsTaken = uint.MaxValue;
        EventManager.Disable<EnergizedUpdateTickedEvent>();
    }
}
