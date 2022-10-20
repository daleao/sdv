namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using System.Xml.Serialization;
using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Extensions.Stardew;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Attacks on-hit reduce defense and destroy enemy armor.</summary>
[XmlType("Mods_DaLion_CarvingEnchantment")]
public class CarvingEnchantment : BaseWeaponEnchantment
{
    private static readonly Lazy<Func<RockCrab, NetBool>> GetShellGone = new(() =>
        typeof(RockCrab)
            .RequireField("shellGone")
            .CompileUnboundFieldGetterDelegate<RockCrab, NetBool>());

    /// <inheritdoc />
    public override string GetName()
    {
        return ModEntry.i18n.Get("enchantments.carving");
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
                crab.Increment(DataFields.Carved);
                if (crab.Read<int>(DataFields.Carved) > 3)
                {
                    GetShellGone.Value(crab).Value = true;
                }

                break;
        }
    }
}
