namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class TreeUpdateTapperProductPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="TreeUpdateTapperProductPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal TreeUpdateTapperProductPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Tree>(nameof(Tree.UpdateTapperProduct));
    }

    #region harmony patches

    /// <summary>Patch to decrease syrup production time for Tapper.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    // ReSharper disable once InconsistentNaming
    private static void TreeUpdateTapperProductPostfix(Tree __instance, SObject? tapper)
    {
        if (tapper?.heldObject.Value is null || (__instance.treeType.Value == Tree.mushroomTree && Game1.currentSeason == "winter"))
        {
            return;
        }

        if (Config.AgingTreesQualitySyrups)
        {
            tapper.heldObject.Value.Quality = __instance.GetQualityFromAge();
        }

        if (tapper.QualifiedItemId == QIDs.HeavyTapper && Config.ImmersiveHeavyTapperYield)
        {
            tapper.heldObject.Value.Stack = 2;
        }

        var owner = tapper.GetOwner();
        if (!owner.HasProfessionOrLax(Profession.Tapper))
        {
            return;
        }

        if (tapper.MinutesUntilReady > 0)
        {
            tapper.MinutesUntilReady = (int)(tapper.MinutesUntilReady *
                                                      (owner.HasProfession(Profession.Tapper, true) ? 0.5 : 0.75));
        }
    }

    #endregion harmony patches
}
