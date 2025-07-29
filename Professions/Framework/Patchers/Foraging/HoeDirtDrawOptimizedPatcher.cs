namespace DaLion.Professions.Framework.Patchers.Foraging;

#region using directives

using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class HoeDirtDrawOptimizedPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="HoeDirtDrawOptimizedPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal HoeDirtDrawOptimizedPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<HoeDirt>(nameof(HoeDirt.DrawOptimized));
    }

    #region harmony patches

    [HarmonyPrefix]
    [UsedImplicitly]
    private static void HoeDirtDrawOptimizedPostfix(HoeDirt __instance, SpriteBatch? dirt_batch)
    {
        if (dirt_batch is null || State.ScavengerHunt is not { IsActive: true } hunt)
        {
            return;
        }

        var directionToTreasure = (hunt.TargetTile.Value - __instance.Tile).ToFacingDirection();
        var drawPosition = Game1.GlobalToLocal(Game1.viewport, __instance.Tile * Game1.tileSize);

        var location = __instance.Location;
        var sourceRect = new Rectangle(0, 0, 7, 9);
        if (location.Name == "Mountain")
        {
            sourceRect.X = 7;
        }
        else if (location.GetSeason() == Season.Winter && !location.SeedsIgnoreSeasonsHere())
        {
            sourceRect.X = 14;
        }

        dirt_batch.Draw(
            Textures.DirtArrow,
            drawPosition + new Vector2(32f, 32f),
            sourceRect,
            Color.White,
            directionToTreasure.Radians() + (float)(Math.PI / 2f),
            new Vector2(3, 4),
            Game1.pixelZoom,
            SpriteEffects.None,
            1.1E-08f);
    }

    #endregion harmony patches
}
