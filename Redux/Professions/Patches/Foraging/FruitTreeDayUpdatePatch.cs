namespace DaLion.Redux.Professions.Patches.Foraging;

#region using directives

using DaLion.Redux.Professions.Extensions;
using HarmonyLib;
using StardewValley.TerrainFeatures;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeDayUpdatePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeDayUpdatePatch"/> class.</summary>
    internal FruitTreeDayUpdatePatch()
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.dayUpdate));
    }

    #region harmony patches

    /// <summary>Patch to increase Abrorist fruit tree growth speed.</summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.HigherThanNormal)]
    private static void FruitTreeDayUpdatePostfix(FruitTree __instance)
    {
        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Arborist, out _) &&
            __instance.daysUntilMature.Value % 4 == 0)
        {
            --__instance.daysUntilMature.Value;
        }
    }

    #endregion harmony patches
}
