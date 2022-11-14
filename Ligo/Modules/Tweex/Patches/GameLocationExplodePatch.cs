namespace DaLion.Ligo.Modules.Tweex.Patches;

#region using directives

using System.Linq;
using DaLion.Shared.Classes;
using HarmonyLib;
using Microsoft.Xna.Framework;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationExplodePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationExplodePatch"/> class.</summary>
    internal GameLocationExplodePatch()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.explode));
    }

    #region harmony patches

    /// <summary>Explosions trigger nearby bombs.</summary>
    [HarmonyPostfix]
    private static void GameLocationExplodePostfix(GameLocation __instance, Vector2 tileLocation, int radius)
    {
        if (!ModEntry.Config.Tweex.ExplosionTriggeredBombs)
        {
            return;
        }

        var circle = new CircleTileGrid(tileLocation, radius * 2);
        foreach (var sprite in __instance.TemporarySprites.Where(sprite =>
                     sprite.bombRadius > 0 && circle.Tiles.Contains(sprite.Position / 64f)))
        {
            sprite.currentNumberOfLoops = Math.Max(sprite.totalNumberOfLoops - 1, sprite.currentNumberOfLoops);
        }
    }

    #endregion harmony patches
}
