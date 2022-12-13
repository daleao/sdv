namespace DaLion.Overhaul.Modules.Arsenal.Patchers;

#region using directives

using System.Reflection;
using DaLion.Overhaul.Modules.Arsenal.Enchantments;
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
        try
        {
            switch (__instance)
            {
                case MeleeWeapon weapon when ArsenalModule.Config.Weapons.RebalancedStats:
                    __result = weapon.getItemLevel() switch
                    {
                        >= 6 => 3,
                        >= 4 => 2,
                        >= 2 => 1,
                        _ => 0,
                    };
                    break;
                case Slingshot when ArsenalModule.Config.Slingshots.AllowForges:
                    __result = __instance.InitialParentTileIndex switch
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
