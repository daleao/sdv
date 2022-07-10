namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using StardewValley;
using StardewValley.Monsters;

#endregion using directives

public class SpoilerEnchantment : BaseWeaponEnchantment
{
    protected override void _OnMonsterSlay(Monster m, GameLocation location, Farmer who)
    {
        if (Game1.random.NextDouble() < 0.2)
            location.monsterDrop(m, m.getTileX(), m.getTileY(), who);
    }

    public override string GetName()
    {
        return "Spoiler";
    }
}