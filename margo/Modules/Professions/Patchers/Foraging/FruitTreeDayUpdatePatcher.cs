namespace DaLion.Overhaul.Modules.Professions.Patchers.Foraging;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeDayUpdatePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeDayUpdatePatcher"/> class.</summary>
    internal FruitTreeDayUpdatePatcher()
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static void FruitTreeDayUpdatePostfix(FruitTree __instance)
    {
        if (__instance.Read<bool>(DataKeys.PlantedByArborist) && __instance.daysUntilMature.Value % 4 == 0)
        {
            __instance.daysUntilMature.Value--;
        }
    }

    #endregion harmony patches
}
