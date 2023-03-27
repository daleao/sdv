namespace DaLion.Overhaul.Modules.Enchantments.Melee;

#region using directives

using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>
///     Absorbs and stores the energy of enemy hits (before mitigation). The next special move releases
///     twice the energy accumulated in an explosion.
/// </summary>
/// <remarks>6 charges per hit + 1 charge per 6 tiles traveled.</remarks>
[XmlType("Mods_DaLion_ExplodingEnchantment")]
public class ExplodingEnchantment : BaseWeaponEnchantment
{
    private int _accumulation;
    private bool _doingExplosion;

    internal int Accumulated
    {
        get => this._accumulation;
        set
        {
            this._accumulation = Math.Min(value, 100);
        }
    }

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.exploding");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (this.Accumulated < 20 || who.CurrentTool is not MeleeWeapon { isOnSpecial: true } weapon ||
            this._doingExplosion)
        {
            return;
        }

        var damage = this.Accumulated * 2;
        var radius = this.Accumulated / 20;
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

        this.Accumulated = 0;
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        base._OnUnequip(who);
        if (!who.IsLocalPlayer)
        {
            return;
        }

        this.Accumulated = 0;
    }
}
