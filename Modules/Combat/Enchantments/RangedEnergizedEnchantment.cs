namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Combat.Projectiles;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Overhaul.Modules.Core.UI;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

/// <summary>
///     Moving and attacking generates Energize stacks, up to 100. At maximum stacks, the next attack causes an electric discharge,
///     dealing heavy damage in a large area.
/// </summary>
/// <remarks>6 charges per hit + 1 charge per 6 tiles traveled.</remarks>
[XmlType("Mods_DaLion_RangedEnergizedEnchantment")]
public sealed class RangedEnergizedEnchantment : BaseSlingshotEnchantment
{
    /// <summary>The amount of energy stacks when fully charged.</summary>
    public const int MaxEnergy = 100;

    private const int BuffSheetIndex = 52;

    private uint _previousStepsTaken;
    private int _energy = -1;
    private bool _doingLightningStrike;

    /// <summary>Finalizes an instance of the <see cref="RangedEnergizedEnchantment"/> class.</summary>
    ~RangedEnergizedEnchantment()
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

    private static int BuffId { get; } = (Manifest.UniqueID + "EnergizedEnchantment").GetHashCode();

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Energized_Name();
    }

    /// <summary>Updates the instance state.</summary>
    /// <param name="ticks">The number of ticks elapsed since the game started, including the current tick.</param>
    public void Update(uint ticks)
    {
        if (ticks % 60 == 0)
        {
            var gained = (Game1.stats.StepsTaken - this._previousStepsTaken) / 3;
            if (gained > 0 && Game1.player.Position != Game1.player.lastPosition)
            {
                this.Energy += (int)gained;
            }

            this._previousStepsTaken = Game1.stats.StepsTaken;
        }

        if (this.Energy <= 0 || Game1.player.hasBuff(BuffId))
        {
            return;
        }

        Game1.buffsDisplay.addOtherBuff(
            new StackableBuff(
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                1,
                "Energized",
                I18n.Enchantments_Energized_Name(),
                () => this.Energy,
                MaxEnergy)
            {
                which = BuffId,
                sheetIndex = BuffSheetIndex,
                millisecondsDuration = 0,
                description = I18n.Enchantments_Energized_Desc(this.Energy),
            });
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (this.Energy < MaxEnergy || this._doingLightningStrike)
        {
            return;
        }

        this.Energy = 0;
        this._doingLightningStrike = true;
        this._doingLightningStrike = false;
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
        //this._energy = 0;
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
        //this._energy = -1;
        EventManager.Disable<EnergizedUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void _OnFire(
        Slingshot slingshot,
        BasicProjectile projectile,
        int damageBase,
        float damageMod,
        float knockback,
        float overcharge,
        Vector2 startingPosition,
        float xVelocity,
        float yVelocity,
        float rotationVelocity,
        GameLocation location,
        Farmer firer)
    {
        if (!firer.IsLocalPlayer)
        {
            return;
        }

        if (projectile is ObjectProjectile @object && this.Energy >= MaxEnergy)
        {
            @object.Energized = true;
            this.Energy = 0;
        }
        else if (!slingshot.Get_IsOnSpecial())
        {
            this.Energy += 4;
        }
    }
}
