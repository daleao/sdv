namespace DaLion.Overhaul.Modules.Tweex.Patchers;

#region using directives

using System.Collections.Immutable;
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

        var tiles = new CircleTileGrid(tileLocation, (uint)radius * 2).Tiles.ToImmutableHashSet();
        for (var i = 0; i < __instance.TemporarySprites.Count; i++)
        {
            var sprite = __instance.TemporarySprites[i];
            if (sprite.bombRadius > 0 && tiles.Contains(sprite.Position / 64f))
            {
                sprite.currentNumberOfLoops = Math.Max(sprite.totalNumberOfLoops - 1, sprite.currentNumberOfLoops);
            }
        }
    }

    #endregion harmony patches
}
