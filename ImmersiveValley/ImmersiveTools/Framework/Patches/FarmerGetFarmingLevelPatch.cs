namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetFarmingLevelPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetFarmingLevelPatch"/> class.</summary>
    internal FarmerGetFarmingLevelPatch()
    {
        this.Target = this.RequireMethod<Farmer>("get_FarmingLevel");
    }

    #region harmony patches

    /// <summary>Master Hoe and Watering Can enchantment effect.</summary>
    [HarmonyPostfix]
    private static void FarmerGetFarmingLevelPostfix(Farmer __instance, ref int __result)
    {
        if (__instance.CurrentTool is { } tool and (Hoe or WateringCan) &&
            tool.hasEnchantmentOfType<MasterEnchantment>())
        {
            ++__result;
        }
    }

    #endregion harmony patches
}
