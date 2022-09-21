namespace DaLion.Stardew.Ponds.Framework.Patches;

#region using directives

using DaLion.Common.Extensions.Stardew;
using HarmonyLib;
using StardewValley.Buildings;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondOnFishTypeChangedPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FishPondOnFishTypeChangedPatch"/> class.</summary>
    internal FishPondOnFishTypeChangedPatch()
    {
        this.Target = this.RequireMethod<FishPond>(nameof(FishPond.OnFishTypeChanged));
    }

    #region harmony patches

    /// <summary>Record pre-reset state.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static void FishPondOnFishTypeChangedPrefix(FishPond __instance, ref int __state)
    {
        __state = __instance.fishType.Value;
    }

    /// <summary>Reset Fish Pond data.</summary>
    [HarmonyPostfix]
    private static void FishPondOnFishTypeChangedPostfix(FishPond __instance, int __state)
    {
        if (__state <= 0 || __instance.fishType.Value > 0)
        {
            return;
        }

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
