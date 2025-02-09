namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class CropNewDayPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CropNewDayPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CropNewDayPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.newDay));
    }

    #region harmony patches

    /// <summary>Patch to record crop planted by Prestiged Agriculturist.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void HoeDirtPlantPostfix(Crop __instance)
    {
        if (Data.ReadAs<bool>(__instance, DataKeys.DaysLeftOutOfSeason))
        {
            Data.Increment(__instance, DataKeys.DaysLeftOutOfSeason, -1);
        }
    }

    #endregion harmony patches
}
