namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using Events;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using System.Xml.Serialization;

#endregion using directives

/// <summary>Moving and attacking generates Energize stacks, up to 100. When fully Energized, the next attack causes an electric discharge.</summary>
/// <remarks>6 charges per hit + 1 charge per 6 tiles traveled.</remarks>
[XmlType("Mods_DaLion_EnergizedEnchantment")]
public class EnergizedEnchantment : BaseWeaponEnchantment
{
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (ModEntry.State.EnergizeStacks >= 100)
        {
            DoLightningStrike(monster, location, who, amount);
            ModEntry.State.EnergizeStacks = 0;
        }
        else
        {
            ModEntry.State.EnergizeStacks += 6;
        }
    }

    protected override void _OnEquip(Farmer who)
    {
        ModEntry.State.EnergizeStacks = 0;
        ModEntry.Events.Enable<EnergizedUpdateTickedEvent>();
    }

    protected override void _OnUnequip(Farmer who)
    {
        ModEntry.State.EnergizeStacks = -1;
        ModEntry.Events.Disable<EnergizedUpdateTickedEvent>();
    }

    public override string GetName() => ModEntry.i18n.Get("enchantments.energized");

    private void DoLightningStrike(Monster monster, GameLocation location, Farmer who, int amount)
    {
        var lightningEvent = new Farm.LightningStrikeEvent
        {
            bigFlash = true,
            createBolt = true,
            boltPosition = monster.Position + new Vector2(32f, 32f)
        };

        Game1.delayedActions.Add(new(200, () =>
        {
            Game1.flashAlpha = (float) (0.5 + Game1.random.NextDouble());
            Game1.playSound("thunder");
            Utility.drawLightningBolt(lightningEvent.boltPosition, location);
            location.damageMonster(new(monster.getTileX() - 6, monster.getTileY() - 6, 12, 12), amount * 3, amount * 5,
                false, who);
        }));
    }
}