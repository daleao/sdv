namespace DaLion.Overhaul.Modules.Combat.Events.Display.RenderedWorld;
using DaLion.Overhaul.Modules.Combat;

#region using directives

using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class BloodthirstyRenderedWorldEvent : RenderedWorldEvent
{
    /// <summary>Initializes a new instance of the <see cref="BloodthirstyRenderedWorldEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal BloodthirstyRenderedWorldEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedWorldImpl(object? sender, RenderedWorldEventArgs e)
    {
        var player = Game1.player;
        if (player.health <= player.maxHealth)
        {
            this.Disable();
            return;
        }

        var position = player.Position -
                       new Vector2(
                           Game1.viewport.X + (int)((Game1.tileSize - (player.Sprite.SpriteWidth * 4 / 3)) * 1.5f),
                           Game1.viewport.Y + (int)((Game1.tileSize + (player.Sprite.SpriteHeight / 2)) * 1.5f));
        var alpha = (player.health - player.maxHealth) / (player.maxHealth * 0.2f);
        e.SpriteBatch.Draw(
            Textures.ShieldTx,
            position,
            null,
            Color.DarkRed * alpha,
            0f,
            Vector2.Zero,
            1.5f,
            SpriteEffects.None,
            1000f);
    }
}
