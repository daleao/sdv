namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Enchantments.Framework.Buffs;
using DaLion.Enchantments.Framework.Events;
using Microsoft.Xna.Framework;
using StardewValley.Tools;

#endregion using directives

/// <summary>
///     Absorbs and stores energy from enemy hits (before mitigation). The next special move releases
///     twice the accumulated energy as an explosion.
/// </summary>
[XmlType("Mods_DaLion_RangedExplosiveEnchantment")]
public sealed class ExplosiveEnchantment : BaseWeaponEnchantment
{
    /// <summary>The highest amount of damage that can be accumulated.</summary>
    public const int MaxAccumulation = 250;

    /// <summary>The amount of damage accumulation required to increase the explosion radius by 1 tile.</summary>
    public const int AccumulationStep = 50;

    private int _accumulation;

    /// <summary>Finalizes an instance of the <see cref="ExplosiveEnchantment"/> class.</summary>
    ~ExplosiveEnchantment()
    {
        EventManager.Disable<ExplosiveUpdateTickedEvent>();
    }

    /// <summary>Gets the largest possible explosion radius.</summary>
    public static int MaxRadius => MaxAccumulation / AccumulationStep;

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

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Explosive_Name();
    }

    /// <summary>Updates the instance state.</summary>
    public void Update()
    {
        if (this.Accumulated < AccumulationStep)
        {
            EventManager.Disable<ExplosiveUpdateTickedEvent>();
            return;
        }

        Game1.player.applyBuff(new ExplosiveBuff(() => this.ExplosionRadius));
    }

    /// <summary>Release stored energy.</summary>
    /// <param name="weapon">The <see cref="MeleeWeapon"/> with this enchantment.</param>
    internal void Explode(MeleeWeapon weapon)
    {
        var who = weapon.lastUser;
        var damage = this.Accumulated * 2;
        var radius = this.ExplosionRadius + 1;
        var toolLocation = who.GetToolLocation();
        var tileLocation = new Vector2((int)toolLocation.X / Game1.tileSize, (int)toolLocation.Y / Game1.tileSize);
        Log.D($"Doing explosion at ({tileLocation.X}, {tileLocation.Y}) with radius {radius} and power {damage}.");

        this.Accumulated = 0;
        who.currentLocation.playSound("explosion");
        who.currentLocation.explode(
            tileLocation,
            radius,
            who,
            true,
            damage,
            true);
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        if (who.IsLocalPlayer)
        {
            EventManager.Disable<ExplosiveUpdateTickedEvent>();
        }
    }
}
