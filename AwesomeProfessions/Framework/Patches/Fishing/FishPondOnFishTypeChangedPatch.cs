namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.Buildings;

using Stardew.Common.Extensions;

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
        if (!ModEntry.Config.EnableFishPondRebalance) return;

        var owner = Game1.getFarmerMaybeOffline(__instance.owner.Value) ?? Game1.MasterPlayer;
        var qualityRatingByFishPond =
            ModData.Read(DataField.QualityRatingByFishPond, owner).ToDictionary<int, int>(",", ";");
        var thisFishPond = __instance.GetCenterTile().ToString().GetDeterministicHashCode();
        if (qualityRatingByFishPond.Remove(thisFishPond))
            ModData.Write(DataField.QualityRatingByFishPond, qualityRatingByFishPond.ToString(",", ";"), owner);
    }

    #endregion harmony patches
}