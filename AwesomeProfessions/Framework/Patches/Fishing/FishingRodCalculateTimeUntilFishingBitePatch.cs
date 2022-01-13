using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Tools;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

[UsedImplicitly]
internal class FishingRodCalculateTimeUntilFishingBitePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishingRodCalculateTimeUntilFishingBitePatch()
    {
        Original = RequireMethod<FishingRod>("calculateTimeUntilFishingBite");
    }

    #region harmony patches

    /// <summary>Patch to reduce prestiged Fisher nibble delay.</summary>
    [HarmonyPrefix]
    private static bool FishingRodCalculateTimeUntilFishingBitePrefux(FishingRod __instance, ref float __result)
    {
        var who = __instance.getLastFarmerToUse();
        if (!who.HasPrestigedProfession("Fisher")) return true; // run original logic

        __result = 50;
        return false; // don't run original logic
    }

    /// <summary>Patch to reduce Fisher nibble delay.</summary>
    [HarmonyPostfix]
    private static void FishingRodCalculateTimeUntilFishingBitePostfix(FishingRod __instance, ref float __result)
    {
        var who = __instance.getLastFarmerToUse();
        if (who.HasProfession("Fisher")) __result *= 0.5f;
    }

    #endregion harmony patches
}