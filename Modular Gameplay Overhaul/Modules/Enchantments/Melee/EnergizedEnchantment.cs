namespace DaLion.Overhaul.Modules.Enchantments.Melee;

#region using directives

using System.Xml.Serialization;
using DaLion.Overhaul.Modules.Enchantments.Events;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewValley.Tools;

#endregion using directives

/// <summary>
///     Moving and attacking generates Energize stacks, up to 100. At maximum stacks, the next attack causes an electric discharge,
///     dealing heavy damage in a large area.
/// </summary>
/// <remarks>6 charges per hit + 1 charge per 6 tiles traveled.</remarks>
[XmlType("Mods_DaLion_EnergizedEnchantment")]
public class EnergizedEnchantment : BaseWeaponEnchantment
{
    internal const int BuffSheetIndex = 42;

    private int _stacks = -1;

    private bool _doingLightningStrike;

    internal static int BuffId { get; } = (Manifest.UniqueID + "Energized").GetHashCode();

    internal int Stacks
    {
        get => this._stacks;
        set
        {
            this._stacks = Math.Min(value, 100);
        }
    }

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Get("enchantments.energized");
    }

    internal void DoLightningStrike(Monster monster, GameLocation location, Farmer who, MeleeWeapon weapon)
    {
        var aoe = monster.GetBoundingBox();
        aoe.Inflate(12 * Game1.tileSize, 12 * Game1.tileSize);
        Game1.flashAlpha = (float)(0.5 + Game1.random.NextDouble());
        Game1.playSound("thunder");
        Utility.drawLightningBolt(monster.Position + new Vector2(32f, 32f), location);
        location.damageMonster(
            aoe,
            weapon.minDamage.Value,
            weapon.maxDamage.Value,
            false,
            who);
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (this._doingLightningStrike)
        {
            return;
        }

        if (this.Stacks >= 100)
        {
            this.Stacks = 0;
            this._doingLightningStrike = true;
            this.DoLightningStrike(monster, location, who, (MeleeWeapon)who.CurrentTool);
            this._doingLightningStrike = false;
        }
        else
        {
            this.Stacks += 6;
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

        this.Stacks = 0;
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

        this.Stacks = -1;
        EventManager.Disable<EnergizedUpdateTickedEvent>();
    }
}
