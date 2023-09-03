namespace DaLion.Overhaul.Modules.Combat.Patchers.Enchantments;

#region using directives

using System.Collections.Generic;
using DaLion.Overhaul.Modules.Combat.VirtualProperties;
using DaLion.Shared.Extensions;
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
        if (item is not MeleeWeapon weapon || !CombatModule.Config.RebalancedGemstones)
        {
            return true; // run original logic
        }

        var data = ModHelper.GameContent
            .Load<Dictionary<int, string>>("Data/weapons")[weapon.InitialParentTileIndex]
            .SplitWithoutAllocation('/');
        weapon.minDamage.Value +=
            (int)(weapon.Read(DataKeys.BaseMinDamage, int.Parse(data[2])) * __instance.GetLevel() * 0.1f);
        weapon.maxDamage.Value +=
            (int)(weapon.Read(DataKeys.BaseMaxDamage, int.Parse(data[3])) * __instance.GetLevel() * 0.1f);
        return false; // don't run original logic
    }

    /// <summary>Reset cached stats.</summary>
    [HarmonyPostfix]
    private static void RubyEnchantmentApplyPostfix(Item item)
    {
        switch (item)
        {
            case MeleeWeapon weapon:
                weapon.Invalidate();
                break;
            case Slingshot slingshot:
                slingshot.Invalidate();
                break;
        }
    }

    #endregion harmony patches
}
