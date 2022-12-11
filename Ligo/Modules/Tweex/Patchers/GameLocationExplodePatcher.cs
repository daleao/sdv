namespace DaLion.Ligo.Modules.Tweex.Patchers;

#region using directives

using System.Linq;
using DaLion.Shared.Classes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationExplodePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationExplodePatcher"/> class.</summary>
    internal GameLocationExplodePatcher()
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.explode));
    }

    #region harmony patches

    /// <summary>Explosions trigger nearby bombs.</summary>
    [HarmonyPostfix]
    private static void GameLocationExplodePostfix(GameLocation __instance, Vector2 tileLocation, int radius)
    {
        if (!TweexModule.Config.ExplosionTriggeredBombs)
        {
            return;
        }

        var circle = new CircleTileGrid(tileLocation, (uint)radius * 2);
        foreach (var sprite in __instance.TemporarySprites.Where(sprite =>
                     sprite.bombRadius > 0 && circle.Tiles.Contains(sprite.Position / 64f)))
        {
            sprite.currentNumberOfLoops = Math.Max(sprite.totalNumberOfLoops - 1, sprite.currentNumberOfLoops);
        }
    }

    #endregion harmony patches
}
