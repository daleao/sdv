namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondOnFishTypeChangedPatch : BasePatch
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

        __instance.WriteData("FishQualities", null);
        __instance.WriteData("FamilyQualities", null);
        __instance.WriteData("FamilyLivingHere", null);
        __instance.WriteData("DaysEmpty", 0.ToString());
        __instance.WriteData("SeaweedLivingHere", null);
        __instance.WriteData("GreenAlgaeLivingHere", null);
        __instance.WriteData("WhiteAlgaeLivingHere", null);
        __instance.WriteData("CheckedToday", null);
        __instance.WriteData("ItemsHeld", null);
    }

    #endregion harmony patches
}