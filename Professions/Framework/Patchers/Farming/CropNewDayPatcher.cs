namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Harmony;
using HarmonyLib;

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

    [HarmonyPrefix]
    [UsedImplicitly]
    private static void CropNewDayPrefix(Crop __instance, ref bool __state)
    {
        __state = __instance.Dirt.readyForHarvest();
    }

    /// <summary>Patch to deduct out-of-season days from Agriculturist crops.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CropNewDayPostfix(Crop __instance, bool __state)
    {
        var soil = __instance.Dirt;
        if (!__state && soil.readyForHarvest())
        {
            var cropId = __instance.GetData().HarvestItemId;
            if (cropId is not null)
            {
                var soilMemory = Data.Read(soil, DataKeys.SoilMemory).ParseList<string>();
                soilMemory.RemoveAll(string.IsNullOrEmpty);
                soilMemory.AddOrReplace(cropId);
                Data.Write(soil, DataKeys.SoilMemory, string.Join(',', soilMemory.TakeLast(4)));
            }
        }

        if (__instance.GetData()?.Seasons.Contains(Game1.season) ?? true)
        {
            return;
        }

        var daysOutOfSeason = Data.ReadAs<int>(__instance, DataKeys.DaysLeftOutOfSeason);
        if (daysOutOfSeason > 0)
        {
            Data.Increment(__instance, DataKeys.DaysLeftOutOfSeason, -1);
        }
    }

    #endregion harmony patches
}
