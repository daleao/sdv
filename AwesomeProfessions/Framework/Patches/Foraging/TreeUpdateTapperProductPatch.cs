namespace DaLion.Stardew.Professions.Framework.Patches.Foraging;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;

using Extensions;

using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal class TreeUpdateTapperProductPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal TreeUpdateTapperProductPatch()
    {
        Original = RequireMethod<Tree>(nameof(Tree.UpdateTapperProduct));
    }

    #region harmony patches

    /// <summary>Patch to decrease syrup production time for Tapper.</summary>
    [HarmonyPostfix]
    private static void TreeUpdateTapperProductPostfix(SObject tapper_instance)
    {
        if (tapper_instance is null) return;

        var owner = Game1.getFarmerMaybeOffline(tapper_instance.owner.Value) ?? Game1.MasterPlayer;
        if (!owner.HasProfession(Profession.Tapper)) return;

        if (tapper_instance.MinutesUntilReady > 0)
            tapper_instance.MinutesUntilReady = (int) (tapper_instance.MinutesUntilReady *
                                                       (owner.HasProfession(Profession.Tapper, true) ? 0.5 : 0.75));
    }

    #endregion harmony patches
}