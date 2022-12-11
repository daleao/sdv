namespace DaLion.Ligo.Modules.Arsenal.Enchantments;

#region using directives

using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks on-hit steal 5% of enemies' current health. Excess healing is converted into a shield for up
///     to 20% of (the player's) max health, which slowly decays after not dealing or taking damage for 25s.
/// </summary>
[XmlType("Mods_DaLion_BloodthirstyEnchantment")]
public class BloodthirstyEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return i18n.Get("enchantments.vampiric");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        var lifeSteal = Math.Max((int)(monster.Health * 0.05f), 1);
        monster.Health -= lifeSteal;
        who.health = Math.Min(who.health + lifeSteal, (int)(who.maxHealth * 1.2f));
        location.debris.Add(
            new Debris(amount, new Vector2(who.getStandingX(), who.getStandingY()), Color.Lime, 1f, who));
        //Game1.playSound("healSound");
    }
}
