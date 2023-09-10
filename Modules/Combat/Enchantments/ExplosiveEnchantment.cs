namespace DaLion.Overhaul.Modules.Combat.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Combat.Events.GameLoop;
using DaLion.Overhaul.Modules.Core.UI;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>
///     Absorbs and stores energy from enemy hits (before mitigation). The next special move releases
///     the accumulated energy as an explosion.
/// </summary>
/// <remarks>6 charges per hit + 1 charge per 6 tiles traveled.</remarks>
[XmlType("Mods_DaLion_ExplosiveEnchantment")]
public sealed class ExplosiveEnchantment : BaseWeaponEnchantment
{
    /// <summary>The highest amount of damage that can be accumulated.</summary>
    public const int MaxAccumulation = 250;

    /// <summary>The amount of damage accumulation required to increase the explosion radius by 1 tile.</summary>
    public const int AccumulationStep = 50;

    private const int BuffSheetIndex = 53;

    private int _accumulation;
    private bool _doingExplosion;

    /// <summary>Finalizes an instance of the <see cref="ExplosiveEnchantment"/> class.</summary>
    ~ExplosiveEnchantment()
    {
        EventManager.Disable<ExplosiveUpdateTickedEvent>();
    }

    /// <summary>Gets or sets the total accumulated damage.</summary>
    public int Accumulated
    {
        get => this._accumulation;
        set
        {
            this._accumulation = Math.Min(value, MaxAccumulation);
        }
    }

    /// <summary>Gets the current radius of the would-be explosion.</summary>
    public int ExplosionRadius => this.Accumulated / AccumulationStep;

    /// <summary>Gets the largest possible explosion radius.</summary>
    public int MaxRadius => MaxAccumulation / AccumulationStep;

    private static int BuffId { get; } = (Manifest.UniqueID + "Explosive").GetHashCode();

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Explosive_Name();
    }

    /// <summary>Updates the instance state.</summary>
    public void Update()
    {
        if (Game1.player.hasBuff(BuffId))
        {
            return;
        }

        if (this.Accumulated < AccumulationStep)
        {
            EventManager.Disable<ExplosiveUpdateTickedEvent>();
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
                "Explosive",
                this.GetName(),
                () => this.ExplosionRadius,
                this.MaxRadius)
            {
                which = BuffId,
                sheetIndex = BuffSheetIndex,
                millisecondsDuration = 0,
                description = I18n.Enchantments_Explosive_Desc(this.ExplosionRadius),
            });
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (!who.IsLocalPlayer || who.CurrentTool is not MeleeWeapon { isOnSpecial: true } weapon ||
            this.Accumulated < AccumulationStep || this._doingExplosion)
        {
            return;
        }

        var damage = this.Accumulated;
        var radius = this.ExplosionRadius;
        var tileLocation = weapon.type.Value == MeleeWeapon.club
            ? who.GetToolLocation() + new Vector2(32f, 32f)
            : monster.getTileLocation();
        Log.D($"[Arsenal]: Doing explosion at ({tileLocation.X}, {tileLocation.Y}) with radius {radius} and power {damage}.");

        this.Accumulated = 0;
        this._doingExplosion = true;
        location.playSound("explosion");
        location.explode(
            tileLocation,
            radius,
            who,
            false,
            damage);
        this._doingExplosion = false;
    }

    /// <inheritdoc />
    protected override void _OnEquip(Farmer who)
    {
        base._OnEquip(who);
        if (!who.IsLocalPlayer)
        {
            return;
        }

        //this._accumulation = 0;
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        if (!who.IsLocalPlayer)
        {
            return;
        }

        //this._accumulation = -1;
        EventManager.Disable<ExplosiveUpdateTickedEvent>();
    }
}
