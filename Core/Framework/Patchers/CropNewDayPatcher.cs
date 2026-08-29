namespace DaLion.Core.Framework.Patchers;

using DaLion.Shared.Constants;

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

    /// <summary>Patch for Winter Wheat.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool CropNewDayPrefix(Crop __instance)
    {
        if (__instance.indexOfHarvest.Value == "262" && Game1.currentSeason == "winter" &&
            __instance.currentPhase.Value > 0 && Config.WinterWheat)
        {
            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
