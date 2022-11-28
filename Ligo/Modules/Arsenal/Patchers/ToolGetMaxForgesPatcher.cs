namespace DaLion.Ligo.Modules.Arsenal.Patchers;

using System.Reflection;

#region using directives

using DaLion.Ligo.Modules.Arsenal.Enchantments;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class ToolGetMaxForgesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ToolGetMaxForgesPatcher"/> class.</summary>
    internal ToolGetMaxForgesPatcher()
    {
        this.Target = this.RequireMethod<Tool>(nameof(Tool.GetMaxForges));
    }

    #region harmony patches

    /// <summary>Custom forge slots for weapons and slingshots + extra slot for Infinity enchant.</summary>
    [HarmonyPrefix]
    private static bool ToolGetMaxForgesPostfix(Tool __instance, ref int __result)
    {
        if (!ModEntry.Config.Arsenal.Weapons.RebalancedWeapons)
        {
            return true; // run original logic
        }

        try
        {
            switch (__instance)
            {
                case MeleeWeapon weapon:
                    __result = weapon.getItemLevel() switch
                    {
                        >= 6 => 3,
                        >= 4 => 2,
                        >= 2 => 1,
                        _ => 0,
                    };
                    break;
                case Slingshot:
                    __result = __instance.ParentSheetIndex switch
                    {
                        Constants.BasicSlingshotIndex => 1,
                        Constants.MasterSlingshotIndex => 2,
                        Constants.GalaxySlingshotIndex => 3,
                        _ => 0,
                    };
                    break;
                default:
                    return true; // run original logic
            }

            if (__instance.hasEnchantmentOfType<InfinityEnchantment>())
            {
                __result++;
            }

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}
