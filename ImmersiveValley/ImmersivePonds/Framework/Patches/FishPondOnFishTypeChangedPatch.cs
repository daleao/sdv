namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using Common.Extensions.Stardew;
using HarmonyLib;
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

        __instance.Write("FishQualities", null);
        __instance.Write("FamilyQualities", null);
        __instance.Write("FamilyLivingHere", null);
        __instance.Write("DaysEmpty", 0.ToString());
        __instance.Write("SeaweedLivingHere", null);
        __instance.Write("GreenAlgaeLivingHere", null);
        __instance.Write("WhiteAlgaeLivingHere", null);
        __instance.Write("CheckedToday", null);
        __instance.Write("ItemsHeld", null);
    }

    #endregion harmony patches
}