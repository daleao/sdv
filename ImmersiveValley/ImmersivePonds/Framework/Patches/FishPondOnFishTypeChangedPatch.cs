namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common.Data;
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

        ModDataIO.WriteTo(__instance, "FishQualities", null);
        ModDataIO.WriteTo(__instance, "FamilyQualities", null);
        ModDataIO.WriteTo(__instance, "FamilyLivingHere", null);
        ModDataIO.WriteTo(__instance, "DaysEmpty", 0.ToString());
        ModDataIO.WriteTo(__instance, "SeaweedLivingHere", null);
        ModDataIO.WriteTo(__instance, "GreenAlgaeLivingHere", null);
        ModDataIO.WriteTo(__instance, "WhiteAlgaeLivingHere", null);
        ModDataIO.WriteTo(__instance, "CheckedToday", null);
        ModDataIO.WriteTo(__instance, "ItemsHeld", null);
    }

    #endregion harmony patches
}