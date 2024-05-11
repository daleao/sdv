namespace DaLion.Professions.Framework.Patchers.Farming;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class CropIsInSeasonPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CropIsInSeasonPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal CropIsInSeasonPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Crop>(nameof(Crop.IsInSeason), [typeof(GameLocation)]);
    }

    #region harmony patches

    /// <summary>Patch for Prestiged Agriculturist crop endurance.</summary>
    [HarmonyPostfix]
    private static void CropIsInSeasonPostfix(Crop __instance, ref bool __result)
    {
        if (!__result && Data.ReadAs<bool>(__instance, DataKeys.PlantedByPrestigedAgriculturist) &&
            Game1.dayOfMonth == 1)
        {
            __result = true;
        }
    }

    #endregion harmony patches
}
