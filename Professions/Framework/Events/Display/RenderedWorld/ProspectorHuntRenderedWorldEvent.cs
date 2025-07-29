namespace DaLion.Professions.Framework.Events.Display.RenderedWorld;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProspectorHuntRenderedWorldEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ProspectorHuntRenderedWorldEvent(EventManager? manager = null)
    : RenderedWorldEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.ProspectorHunt?.IsActive ?? false;

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        if (State.ProspectorHunt?.TreasureStone is not { } stone)
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

        e.SpriteBatch.Draw(
            texture: itemData.GetTexture(),
            Game1.GlobalToLocal(Game1.viewport, new Vector2((int)(x * Game1.tileSize), (int)(y * Game1.tileSize))),
            sourceRectangle: itemData.GetSourceRect(),
            color,
            0f,
            Vector2.Zero,
            4f,
            stone.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
            999999f);
        new Rectangle((int)x, (int)y, Game1.tileSize, Game1.tileSize).BorderHighlight(Color.Yellow, e.SpriteBatch);
    }
}
