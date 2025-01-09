namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDirtPlantPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDirtPlantPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal HoeDirtPlantPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<HoeDirt>(nameof(HoeDirt.plant));
    }

    #region harmony patches

    /// <summary>Patch to record crop planted by Prestiged Agriculturist.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void HoeDirtPlantPostfix(HoeDirt __instance, Farmer who)
    {
        if (__instance.crop is { } crop && who.HasProfession(Profession.Agriculturist, true))
        {
            Data.Write(crop, DataKeys.PlantedByPrestigedAgriculturist, true.ToString());
        }
    }

    #endregion harmony patches
}
