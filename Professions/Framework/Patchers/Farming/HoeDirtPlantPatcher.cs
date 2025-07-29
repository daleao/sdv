namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDirtPlantPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDirtPlantPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal HoeDirtPlantPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<HoeDirt>(nameof(HoeDirt.plant));
    }

    #region harmony patches

    /// <summary>Patch to record crops planted by Agriculturist.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void HoeDirtPlantPostfix(HoeDirt __instance, Farmer who)
    {
        if (__instance.crop is not { } crop)
        {
            return;
        }

        if (who.HasProfessionOrLax(Profession.Agriculturist, true))
        {
            Data.Write(crop, DataKeys.DaysOutOfSeason, "1");
        }
    }

    #endregion harmony patches
}
