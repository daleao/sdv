namespace DaLion.Professions.Framework.Patchers.Mining;

#region using directives

using DaLion.Shared.Extensions.Stardew;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationDrawPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationDrawPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal GameLocationDrawPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<GameLocation>(nameof(GameLocation.draw), [typeof(SpriteBatch)]);
    }

    #region harmony patches

    [HarmonyPostfix]
    [UsedImplicitly]
    private static void GameLocationDrawPostfix(GameLocation __instance, SpriteBatch b)
    {
        if (State.ProspectorHunt is null || !State.ProspectorHunt.IsActive ||
            !ReferenceEquals(State.ProspectorHunt.Location, __instance) ||
            State.ProspectorHunt.TreasureStone is not { } stone)
        {
            return;
        }

        var itemData = ItemRegistry.GetDataOrErrorItem(stone.QualifiedItemId);
        var (x, y) = State.ProspectorHunt.TargetTile!.Value;
        var t = (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalSeconds * 2d * Math.PI);
        Color color;
        float alpha;
        if (t >= 0)
        {
            alpha = MathHelper.Lerp(0f, 1f, t);
            color = Color.Yellow * alpha;
        }
        else
        {
            alpha = MathHelper.Lerp(0f, 0.4f, -t);
            color = Color.Black * alpha;
        }

        var baseSort = ((y + 1) * 64 / 10000f) + (stone.TileLocation.X / 50000f);
        b.Draw(
            texture: itemData.GetTexture(),
            Game1.GlobalToLocal(Game1.viewport, new Vector2((int)(x * Game1.tileSize), (int)(y * Game1.tileSize))),
            sourceRectangle: itemData.GetSourceRect(),
            color,
            0f,
            Vector2.Zero,
            4f,
            stone.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            baseSort + 2E-05f);
        if (stone.lightSource is not null)
        {
            stone.lightSource.radius.Value = alpha * 2;
        }
    }

    #endregion harmony patches
}
