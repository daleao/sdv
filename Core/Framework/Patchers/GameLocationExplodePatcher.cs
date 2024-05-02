namespace DaLion.Core.Framework.Patchers;

#region using directives

using DaLion.Shared.Classes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationExplodePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationExplodePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal GameLocationExplodePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.explode));
    }

    #region harmony patches

    /// <summary>Patch for Blaster double coal chance + Demolitionist speed burst.</summary>
    [HarmonyPostfix]
    private static void GameLocationExplodePostfix(
        GameLocation __instance, Vector2 tileLocation, int radius, Farmer? who)
    {
        var tiles = new CircleTileGrid(tileLocation, (uint)radius * 2).Tiles.ToHashSet();
        foreach (var sprite in __instance.TemporarySprites)
        {
            if (sprite.bombRadius > 0 && tiles.Contains(sprite.Position / 64f))
            {
                sprite.currentNumberOfLoops = Math.Max(sprite.totalNumberOfLoops - 1, sprite.currentNumberOfLoops);
            }
        }
    }

    #endregion harmony patches
}
