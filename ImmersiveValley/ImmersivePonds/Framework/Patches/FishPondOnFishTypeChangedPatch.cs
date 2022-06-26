namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Common.Data;
using Common.Harmony;

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

        ModDataIO.WriteData(__instance, "FishQualities", null);
        ModDataIO.WriteData(__instance, "FamilyQualities", null);
        ModDataIO.WriteData(__instance, "FamilyLivingHere", null);
        ModDataIO.WriteData(__instance, "DaysEmpty", 0.ToString());
        ModDataIO.WriteData(__instance, "SeaweedLivingHere", null);
        ModDataIO.WriteData(__instance, "GreenAlgaeLivingHere", null);
        ModDataIO.WriteData(__instance, "WhiteAlgaeLivingHere", null);
        ModDataIO.WriteData(__instance, "CheckedToday", null);
        ModDataIO.WriteData(__instance, "ItemsHeld", null);
    }

    #endregion harmony patches
}