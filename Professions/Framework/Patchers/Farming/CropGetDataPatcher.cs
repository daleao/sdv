namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.GameData.Crops;

#endregion using directives

[UsedImplicitly]
internal sealed class CropGetDataPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CropGetDataPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CropGetDataPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.GetData));
    }

    #region harmony patches

    /// <summary>Patch to record crop planted by Prestiged Agriculturist.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void CropGetDataPostfix(Crop __instance, CropData? __result)
    {
        if (__result is null || !Game1.player.HasProfessionOrLax(Profession.Agriculturist, true))
        {
            return;
        }

        var dirt = __instance.Dirt;
        if (dirt is null)
        {
            return;
        }

        var fertilizerSpeedBoost = dirt.GetFertilizerSpeedBoost();
        var cropMemory = Data.Read(dirt.crop, DataKeys.SoilMemory);
        var stacks = 0;
        if (!string.IsNullOrEmpty(cropMemory))
        {
            stacks = cropMemory.Count(c => c == ',') + 1;
        }

        var totalSpeedBoost = (stacks * 0.05f) + fertilizerSpeedBoost;
        __result.RegrowDays = (int)Math.Ceiling(__result.RegrowDays * (1f - totalSpeedBoost));
    }

    #endregion harmony patches
}
