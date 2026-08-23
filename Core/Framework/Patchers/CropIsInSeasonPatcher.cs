namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Constants;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CropIsInSeasonPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CropIsInSeasonPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CropIsInSeasonPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.IsInSeason), [typeof(GameLocation)]);
    }

    #region harmony patches

    /// <summary>Patch for Winter Wheat.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CropIsInSeasonPostfix(Crop __instance, ref bool __result)
    {
        if (__instance.indexOfHarvest.Value == "262" && Game1.currentSeason == "winter" &&
            __instance.currentPhase.Value > 0 && Config.WinterWheat)
        {
            __result = true;
            Data.WriteIfNotExists(__instance, DataKeys.WinterWheat, true.ToString());
        }
    }

    #endregion harmony patches
}
