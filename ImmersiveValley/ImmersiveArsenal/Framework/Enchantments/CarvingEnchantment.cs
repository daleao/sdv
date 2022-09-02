namespace DaLion.Stardew.Arsenal.Framework.Enchantments;

#region using directives

using Common.Extensions.Reflection;
using Common.Extensions.Stardew;
using Netcode;
using StardewValley.Monsters;
using System;
using System.Xml.Serialization;

#endregion using directives

/// <summary>Attacks on-hit reduce defense and destroy enemy armor.</summary>
[XmlType("Mods_DaLion_CarvingEnchantment")]
public class CarvingEnchantment : BaseWeaponEnchantment
{
    private static readonly Lazy<Func<RockCrab, NetBool>> _GetShellGone = new(() =>
        typeof(RockCrab).RequireField("shellGone").CompileUnboundFieldGetterDelegate<RockCrab, NetBool>());

    protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
    {
        monster.resilience.Value = Math.Max(monster.resilience.Value - 1, -1);
        switch (monster)
        {
            case Bug {isArmoredBug.Value: true, resilience.Value: < 0} bug:
                bug.isArmoredBug.Value = false;
                break;
            case RockCrab crab:
                crab.Increment("Carved");
                if (crab.Read<int>("Carved") > 3) _GetShellGone.Value(crab).Value = true;
                break;
        }
    }

    public override string GetName() => ModEntry.i18n.Get("enchantments.carving");
}