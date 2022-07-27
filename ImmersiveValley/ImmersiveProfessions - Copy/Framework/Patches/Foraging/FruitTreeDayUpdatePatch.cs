namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeDayUpdatePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FruitTreeDayUpdatePatch()
    {
        Target = RequireMethod<FruitTree>(nameof(FruitTree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static void FruitTreeDayUpdatePostfix(FruitTree __instance)
    {
        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Arborist, out _) &&
            __instance.daysUntilMature.Value % 4 == 0)
            --__instance.daysUntilMature.Value;
    }

    #endregion harmony patches
}