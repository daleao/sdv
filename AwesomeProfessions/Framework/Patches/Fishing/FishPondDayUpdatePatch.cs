namespace DaLion.Stardew.Professions.Framework.Patches.Fishing;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley.Buildings;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FishPondDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FishPondDayUpdatePatch()
    {
        Original = RequireMethod<FishPond>(nameof(FishPond.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to boost roe production for everybody.</summary>
    [HarmonyPostfix]
    private static void FishPondDayUpdatePostfix(FishPond __instance)
    {
        if (ModEntry.Config.EnableFishPondRebalance) __instance.AddBonusRoeAmountAndQuality();
    }

    #endregion harmony patches
}