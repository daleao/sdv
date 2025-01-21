namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Buildings;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class PondQueryMenuCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="PondQueryMenuCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal PondQueryMenuCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<PondQueryMenu>(typeof(FishPond));
    }

    #region harmony patches

    /// <summary>Patch to adjust fish pond query menu for Aquarist increased max capacity.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void PondQueryMenuDrawPostfix(FishPond fish_pond)
    {
        if (fish_pond.maxOccupants.Value > 10)
        {
            PondQueryMenu.height += 32;
        }
    }

    #endregion harmony patches
}
