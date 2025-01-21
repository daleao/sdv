namespace DaLion.Ponds.Framework.Patchers;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Buildings;

#endregion using directives

[UsedImplicitly]
internal sealed class FishPondCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FishPondCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FishPondCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<FishPond>(typeof(Vector2));
    }

    #region harmony patches

    /// <summary>Compensates for the game calling dayUpdate *twice* immediately upon construction.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FishPondCtorPostfix(FishPond __instance)
    {
        Data.Write(__instance, DataKeys.DaysEmpty, (-3).ToString()); // it's -3 for good measure (and also immersion; a fresh pond takes longer to get dirty)
    }

    #endregion harmony patches
}
