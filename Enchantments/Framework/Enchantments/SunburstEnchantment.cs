namespace DaLion.Enchantments.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks fire a short-range beam of solar energy. Prevents mummies from resurrecting.
/// </summary>
[XmlType("Mods_DaLion_SunburstEnchantment")]
public sealed class SunburstEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return I18n.Enchantments_Sunburst_Name();
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        if (!who.IsLocalPlayer)
        {
            return;
        }

        monster.resilience.Value--;
        switch (monster)
        {
            case Bug { isArmoredBug.Value: true, resilience.Value: < -3 } bug:
                bug.isArmoredBug.Value = false;
                break;
            case RockCrab crab:
                var shellHealth = Reflector
                    .GetUnboundFieldGetter<RockCrab, NetInt>("shellHealth")
                    .Invoke(crab).Value;
                if (shellHealth <= 0)
                {
                    break;
                }

                shellHealth--;
                Reflector
                    .GetUnboundFieldGetter<RockCrab, NetInt>("shellHealth")
                    .Invoke(crab).Value = shellHealth;
                crab.shake(500);
                if (shellHealth <= 0)
                {
                    Reflector
                        .GetUnboundFieldGetter<RockCrab, NetBool>("shellGone")
                        .Invoke(crab).Value = true;
                    crab.moveTowardPlayer(-1);
                    location.playSound("stoneCrack");
                    Game1.createRadialDebris(location, 14, (int)crab.Tile.X, (int)crab.Tile.Y, Game1.random.Next(2, 7), resource: false);
                    Game1.createRadialDebris(location, 14, (int)crab.Tile.X, (int)crab.Tile.Y, Game1.random.Next(2, 7), resource: false);
                }

                break;
        }
    }
}
