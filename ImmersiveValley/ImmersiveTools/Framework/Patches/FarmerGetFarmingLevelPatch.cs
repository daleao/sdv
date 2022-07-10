namespace DaLion.Stardew.Tools.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Tools;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetFarmingLevelPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerGetFarmingLevelPatch()
    {
        Target = RequireMethod<Farmer>("get_FarmingLevel");
    }

    #region harmony patches

    /// <summary>Master Hoe and Watering Can enchantment effect.</summary>
    [HarmonyPostfix]
    private static void FarmerGetFarmingLevelPostfix(Farmer __instance, ref int __result)
    {
        if (__instance.CurrentTool is { } tool and (Hoe or WateringCan) && tool.hasEnchantmentOfType<MasterEnchantment>())
            ++__result;
    }

    #endregion harmony patches
}