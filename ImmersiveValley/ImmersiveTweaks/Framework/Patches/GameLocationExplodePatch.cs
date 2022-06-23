namespace DaLion.Stardew.Tweex.Framework.Patches;

#region using directives

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;

using Common.Classes;
using Common.Harmony;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationExplodePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal GameLocationExplodePatch()
    {
        Target = RequireMethod<GameLocation>(nameof(GameLocation.explode));
    }

    #region harmony patches

    /// <summary>Explosions trigger nearby bombs.</summary>
    [HarmonyPostfix]
    private static void GameLocationExplodePostfix(GameLocation __instance, Vector2 tileLocation, int radius)
    {
        if (!ModEntry.Config.ExplosionTriggeredBombs) return;

        var circle = new CircleTileGrid(tileLocation, radius * 2);
        foreach (var sprite in __instance.TemporarySprites.Where(sprite => sprite.bombRadius > 0 && circle.Tiles.Contains(sprite.Position / 64f)))
            sprite.currentNumberOfLoops = sprite.totalNumberOfLoops;
    }

    #endregion harmony patches
}