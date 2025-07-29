namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

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

    /// <summary>Patch to record crop planted by Prestiged Agriculturist.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CropNewDayPostfix(Crop __instance)
    {
        if (__instance.GetData()?.Seasons.Contains(Game1.season) ?? true)
        {
            return;
        }

        var daysOutOfSeason = Data.ReadAs(__instance, DataKeys.DaysOutOfSeason, -1);
        switch (daysOutOfSeason)
        {
            case >= 7:
                Data.Write(__instance, DataKeys.DaysOutOfSeason, null);
                break;
            case >= 0:
                Data.Increment(__instance, DataKeys.DaysOutOfSeason);
                break;
        }
    }

    #endregion harmony patches
}
