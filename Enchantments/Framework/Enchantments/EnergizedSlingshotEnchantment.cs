namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Core.Framework.Enchantments;
using DaLion.Enchantments.Framework.Buffs;
using DaLion.Enchantments.Framework.Events;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>
///     Moving and attacking generates energy, up to 100 stacks. At maximum stacks, the next shot carries an electric charge,
///     which discharges dealing area heavy damage when it hits an enemy.
/// </summary>
/// <remarks>6 charges per hit + 1 charge per 3 tiles traveled.</remarks>
[XmlType("Mods_DaLion_EnergizedSlingshotEnchantment")]
public sealed class EnergizedSlingshotEnchantment : BaseSlingshotEnchantment
{
    /// <summary>The amount of energy stacks when fully charged.</summary>
    public const int MaxEnergy = 100;

    private uint _previousStepsTaken;
    private int _energy = -1;

    /// <summary>Finalizes an instance of the <see cref="EnergizedSlingshotEnchantment"/> class.</summary>
    ~EnergizedSlingshotEnchantment()
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
        if (ticks % 60 == 0)
        {
            var gained = (Game1.stats.StepsTaken - this._previousStepsTaken) / 3;
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
    public override void OnFire(
        Slingshot slingshot,
        BasicProjectile firedProjectile,
        GameLocation location,
        Farmer firer)
    {
        if (!firer.IsLocalPlayer)
        {
            return;
        }

        if (this.Energy >= MaxEnergy)
        {
            Data.Write(firedProjectile, DataKeys.Energized, true.ToString());
            this.Energy = 0;
            SoundBox.PlasmaShot.PlayAll(location);
        }
        else
        {
            this.Energy += 6;
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
