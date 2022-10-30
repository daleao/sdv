﻿namespace DaLion.Redux.Professions.Patches.Fishing;

#region using directives

using DaLion.Redux.Professions.Extensions;
using HarmonyLib;
using StardewValley.Tools;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FishingRodCalculateTimeUntilFishingBitePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FishingRodCalculateTimeUntilFishingBitePatch"/> class.</summary>
    internal FishingRodCalculateTimeUntilFishingBitePatch()
    {
        this.Target = this.RequireMethod<FishingRod>("calculateTimeUntilFishingBite");
    }

    #region harmony patches

    /// <summary>Patch to reduce prestiged Fisher nibble delay.</summary>
    [HarmonyPrefix]
    private static bool FishingRodCalculateTimeUntilFishingBitePrefix(FishingRod __instance, ref float __result)
    {
        var who = __instance.getLastFarmerToUse();
        if (!who.HasProfession(Profession.Fisher, true))
        {
            return true; // run original logic
        }

        __result = 50;
        return false; // don't run original logic
    }

    /// <summary>Patch to reduce Fisher nibble delay.</summary>
    [HarmonyPostfix]
    private static void FishingRodCalculateTimeUntilFishingBitePostfix(FishingRod __instance, ref float __result)
    {
        var who = __instance.getLastFarmerToUse();
        if (who.HasProfession(Profession.Fisher))
        {
            __result *= 0.5f;
        }
    }

    #endregion harmony patches
}