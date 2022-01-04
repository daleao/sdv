using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches.Foraging;

[UsedImplicitly]
internal class FruitTreeDayUpdatePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FruitTreeDayUpdatePatch()
    {
        Original = RequireMethod<FruitTree>(nameof(FruitTree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
    [HarmonyPostfix]
    private static void FruitTreeDayUpdatePostfix(ref FruitTree __instance)
    {
        if (Game1.game1.DoesAnyPlayerHaveProfession("Arborist", out _) &&
            __instance.daysUntilMature.Value % 4 == 0)
            --__instance.daysUntilMature.Value;
    }

    #endregion harmony patches
}