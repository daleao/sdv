namespace DaLion.Overhaul.Modules.Tools.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetFishingLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetFishingLevelPatcher"/> class.</summary>
    internal FarmerGetFishingLevelPatcher()
    {
        this.Target = this.RequirePropertyGetter<Farmer>(nameof(Farmer.FishingLevel));
    }

    #region harmony patches

    /// <summary>Restrict Master enchantment's effect on Fishing.</summary>
    [HarmonyPrefix]
    // ReSharper disable once RedundantAssignment
    private static bool FarmerGetMiningLevelPrefix(Farmer __instance, ref int __result)
    {
        __result = __instance.fishingLevel.Value + __instance.addedFishingLevel.Value;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
