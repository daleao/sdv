namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class FruitTreeShakePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FruitTreeShakePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FruitTreeShakePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<FruitTree>(nameof(FruitTree.shake));
    }

    #region harmony patches

    /// <summary>Patch to apply Ecologist perk to shaken fruit trees.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static void FruitTreeGetQualityPrefix(FruitTree __instance)
    {
        if (!Game1.player.HasProfession(Profession.Ecologist))
        {
            return;
        }

        foreach (var fruit in __instance.fruit)
        {
            Data.AppendToEcologistItemsForaged(fruit.ItemId);
        }
    }

    #endregion harmony patches
}
