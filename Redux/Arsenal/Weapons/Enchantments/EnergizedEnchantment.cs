namespace DaLion.Redux.Arsenal.Weapons.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Redux.Arsenal.Weapons.Events;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Moving and attacking generates Energize stacks, up to 100. When fully Energized, the next attack causes an
///     electric discharge.
/// </summary>
/// <remarks>6 charges per hit + 1 charge per 6 tiles traveled.</remarks>
[XmlType("Mods_DaLion_EnergizedEnchantment")]
public class EnergizedEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return ModEntry.i18n.Get("enchantments.energized");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (ModEntry.State.Arsenal.EnergizeStacks >= 100)
        {
            this.DoLightningStrike(monster, location, who, amount);
            ModEntry.State.Arsenal.EnergizeStacks = 0;
        }
        else
        {
            ModEntry.State.Arsenal.EnergizeStacks += 6;
        }
    }

    /// <inheritdoc />
    protected override void _OnEquip(Farmer who)
    {
        ModEntry.State.Arsenal.EnergizeStacks = 0;
        ModEntry.Events.Enable<EnergizedUpdateTickedEvent>();
    }

    /// <inheritdoc />
    protected override void _OnUnequip(Farmer who)
    {
        ModEntry.State.Arsenal.EnergizeStacks = -1;
        ModEntry.Events.Disable<EnergizedUpdateTickedEvent>();
    }

    private void DoLightningStrike(Monster monster, GameLocation location, Farmer who, int amount)
    {
        var lightningEvent = new Farm.LightningStrikeEvent
        {
            bigFlash = true,
            createBolt = true,
            boltPosition = monster.Position + new Vector2(32f, 32f),
        };

        Game1.delayedActions.Add(new DelayedAction(200, () =>
        {
            Game1.flashAlpha = (float)(0.5 + Game1.random.NextDouble());
            Game1.playSound("thunder");
            Utility.drawLightningBolt(lightningEvent.boltPosition, location);
            location.damageMonster(
                new Rectangle(monster.getTileX() - 6, monster.getTileY() - 6, 12, 12),
                amount * 3,
                amount * 5,
                false,
                who);
        }));
    }
}
