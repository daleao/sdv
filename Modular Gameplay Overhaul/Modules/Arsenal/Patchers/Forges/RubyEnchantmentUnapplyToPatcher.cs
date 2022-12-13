namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Forges;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RubyEnchantmentUnapplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RubyEnchantmentUnapplyToPatcher"/> class.</summary>
    internal RubyEnchantmentUnapplyToPatcher()
    {
        this.Target = this.RequireMethod<RubyEnchantment>("_UnapplyTo");
    }

    #region harmony patches

    /// <summary>Adjust Ruby enchant for randomized weapons.</summary>
    [HarmonyPrefix]
    private static bool RubyEnchantmentUnapplyToPrefix(RubyEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ArsenalModule.Config.Weapons.RebalancedStats)
        {
            return true; // run original logic
        }

        var data = ModHelper.GameContent
            .Load<Dictionary<int, string>>("Data/weapons")[weapon.InitialParentTileIndex]
            .Split('/');
        weapon.minDamage.Value -=
            (int)Math.Min(
                weapon.Read(DataFields.BaseMinDamage, Convert.ToInt32(data[2])) * __instance.GetLevel() * 0.1f, 1);
        weapon.maxDamage.Value -=
            (int)Math.Min(
                weapon.Read(DataFields.BaseMaxDamage, Convert.ToInt32(data[3])) * __instance.GetLevel() * 0.1f, 1);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
