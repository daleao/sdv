namespace DaLion.Professions.Framework.Patchers.Fishing;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;

#endregion using directives

[UsedImplicitly]
internal sealed class CrabPotIsValidCrabPotLocationTilePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="CrabPotIsValidCrabPotLocationTilePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal CrabPotIsValidCrabPotLocationTilePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<CrabPot>(nameof(CrabPot.IsValidCrabPotLocationTile));
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void IsValidCrabPotLocationTile(ref bool __result, GameLocation location, int x, int y)
    {
        if (__result)
        {
            return;
        }

        if (location is not Caldera || !Game1.player.HasProfession(Profession.Trapper, true))
        {
            return;
        }

        var placementTile = new Vector2(x, y);
        var neighborCheck = (location.IsWaterOrLavaTile(x + 1, y) && location.IsWaterOrLavaTile(x - 1, y)) ||
            (location.IsWaterOrLavaTile(x, y + 1) && location.IsWaterOrLavaTile(x, y - 1));
        if (location.objects.ContainsKey(placementTile) || !neighborCheck ||
            !location.IsWaterOrLavaTile((int)placementTile.X, (int)placementTile.Y) ||
            location.doesTileHaveProperty((int)placementTile.X, (int)placementTile.Y, "Passable", "Buildings") != null)
        {
            __result = false;
        }

        __result = true;
    }

    #endregion harmony patches
}
