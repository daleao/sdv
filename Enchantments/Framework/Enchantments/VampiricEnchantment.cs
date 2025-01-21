namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Enchantments.Framework.Animations;
using DaLion.Shared.Extensions;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks on-hit heal for 10% of damage dealt. Excess healing is converted into a shield for up
///     to 20% of (the player's) max health, which slowly decays after not dealing or taking damage for 15s.
/// </summary>
[XmlType("Mods_DaLion_VampiricEnchantment")]
public sealed class VampiricEnchantment : BaseWeaponEnchantment
{
    private Random _random = new(Guid.NewGuid().GetHashCode());

    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Vampiric_Name();
    }

    /// <inheritdoc />
    public override void OnMonsterSlay(Monster m, GameLocation location, Farmer who, bool slainByBomb)
    {
        base.OnMonsterSlay(m, location, who, slainByBomb);
        var lifeSteal = Math.Max((int)(m.MaxHealth * this._random.NextFloat(0.01f, 0.05f)), 1);
        who.health = Math.Min(who.health + lifeSteal, (int)(who.maxHealth * 1.2f));
        location.debris.Add(new Debris(
            lifeSteal,
            new Vector2(who.StandingPixel.X, who.StandingPixel.Y),
            Color.Lime,
            1f,
            who));
        Game1.playSound("healSound");
        Log.D($"{who.Name} absorbed {lifeSteal} health.");
        if (who.health > who.maxHealth)
        {
            ShieldAnimation.Instance = new ShieldAnimation(who);
        }
    }
}
