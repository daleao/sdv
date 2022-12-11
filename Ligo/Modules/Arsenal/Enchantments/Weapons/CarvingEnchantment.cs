namespace DaLion.Ligo.Modules.Arsenal.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Ligo.Modules.Arsenal.VirtualProperties;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

/// <summary>
///     Attacks on-hit reduce enemy defense, down to a minimum of -1. Armored enemies (i.e., Armored Bugs and shelled Rock Crabs)
///     lose their armor upon hitting 0 defense.
/// </summary>
[XmlType("Mods_DaLion_CarvingEnchantment")]
public class CarvingEnchantment : BaseWeaponEnchantment
{
    /// <inheritdoc />
    public override string GetName()
    {
        return i18n.Get("enchantments.carving");
    }

    /// <inheritdoc />
    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        monster.resilience.Value = Math.Max(monster.resilience.Value - 1, -1);
        switch (monster)
        {
            case Bug { isArmoredBug.Value: true, resilience.Value: < 0 } bug:
                bug.isArmoredBug.Value = false;
                break;
            case RockCrab crab:
                crab.Increment_Carved();
                if (crab.Get_Carved() > 3)
                {
                    Reflector
                        .GetUnboundFieldGetter<RockCrab, NetBool>(crab, "shellGone")
                        .Invoke(crab).Value = true;
                }

                break;
        }
    }
}
