namespace DaLion.Overhaul.Modules.Arsenal.Patchers.Forges;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class RubyEnchantmentApplyToPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="RubyEnchantmentApplyToPatcher"/> class.</summary>
    internal RubyEnchantmentApplyToPatcher()
    {
        this.Target = this.RequireMethod<RubyEnchantment>("_ApplyTo");
    }

    #region harmony patches

    /// <summary>Adjust Ruby enchant for randomized weapons.</summary>
    [HarmonyPrefix]
    private static bool RubyEnchantmentApplyToPrefix(RubyEnchantment __instance, Item item)
    {
        if (item is not MeleeWeapon weapon || !ArsenalModule.Config.Weapons.RebalancedWeapons)
        {
            return true; // run original logic
        }

        var data = ModHelper.GameContent
            .Load<Dictionary<int, string>>("Data/weapons")[weapon.InitialParentTileIndex]
            .Split('/');
        weapon.minDamage.Value +=
            (int)Math.Min(
                weapon.Read(DataFields.BaseMinDamage, Convert.ToInt32(data[2])) * __instance.GetLevel() * 0.1f, 1);
        weapon.maxDamage.Value +=
            (int)Math.Min(
                weapon.Read(DataFields.BaseMaxDamage, Convert.ToInt32(data[3])) * __instance.GetLevel() * 0.1f, 1);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
