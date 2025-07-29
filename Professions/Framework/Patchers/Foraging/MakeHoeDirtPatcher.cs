namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class MakeHoeDirtPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="MakeHoeDirtPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal MakeHoeDirtPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.makeHoeDirt));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void MakeHoeDirtPostfix(GameLocation __instance, Vector2 tileLocation, ref bool ignoreChecks)
    {
        var hunt = State.ScavengerHunt;
        if ((hunt?.IsActive ?? false) && ReferenceEquals(__instance, hunt.Location) && tileLocation == hunt.TargetTile &&
            __instance.terrainFeatures.TryGetValue(hunt.TargetTile.Value, out var feature) && feature is HoeDirt)
        {
            hunt.Complete();
        }
    }

    #endregion harmony patches
}
