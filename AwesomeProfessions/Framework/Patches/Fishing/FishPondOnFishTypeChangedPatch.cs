namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishPondOnFishTypeChangedPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondOnFishTypeChangedPatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.OnFishTypeChanged));
    }

    #region harmony patches

    /// <summary>Patch to reset total Fish Pond quality rating.</summary>
    [HarmonyPostfix]
    private static void FishPondOnFishTypeChangedPostfix(FishPond __instance)
    {
        if (!ModEntry.Config.RebalanceFishPonds) return;

        __instance.WriteData("QualityRating", null);
        __instance.WriteData("FamilyQualityRating", null);
        __instance.WriteData("FamilyCount", null);
        __instance.WriteData("DaysEmpty", 0.ToString());
    }

    #endregion harmony patches
}