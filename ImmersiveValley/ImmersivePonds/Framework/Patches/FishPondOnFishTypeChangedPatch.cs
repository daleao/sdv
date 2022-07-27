namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common.ModData;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondOnFishTypeChangedPatch : Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondOnFishTypeChangedPatch()
    {
        Target = RequireMethod<FishPond>(nameof(FishPond.OnFishTypeChanged));
    }

    #region harmony patches

    /// <summary>Reset Fish Pond data.</summary>
    [HarmonyPostfix]
    private static void FishPondOnFishTypeChangedPostfix(FishPond __instance)
    {
        if (__instance.fishType.Value > 0) return;

        ModDataIO.Write(__instance, "FishQualities", null);
        ModDataIO.Write(__instance, "FamilyQualities", null);
        ModDataIO.Write(__instance, "FamilyLivingHere", null);
        ModDataIO.Write(__instance, "DaysEmpty", 0.ToString());
        ModDataIO.Write(__instance, "SeaweedLivingHere", null);
        ModDataIO.Write(__instance, "GreenAlgaeLivingHere", null);
        ModDataIO.Write(__instance, "WhiteAlgaeLivingHere", null);
        ModDataIO.Write(__instance, "CheckedToday", null);
        ModDataIO.Write(__instance, "ItemsHeld", null);
    }

    #endregion harmony patches
}