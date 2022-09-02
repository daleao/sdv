namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly]
internal sealed class DesperadoRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DesperadoRenderedHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        var lastUser = Game1.player;
        if (lastUser.CurrentTool is not Slingshot slingshot || !lastUser.usingSlingshot ||
            lastUser.get_Ultimate() is DeathBlossom { IsActive: true }) return;

        var overcharge = slingshot.GetOvercharge(Game1.player);
        if (overcharge <= 0f) return;

        e.SpriteBatch.Draw(Game1.mouseCursors,
            Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-48f, -160f)),
            new(193, 1868, 47, 12), Color.White, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.885f);

        e.SpriteBatch.Draw(Game1.staminaRect,
            new((int)Game1.GlobalToLocal(Game1.viewport, lastUser.Position).X - 36,
                (int)Game1.GlobalToLocal(Game1.viewport, lastUser.Position).Y - 148, (int)(164f * overcharge), 25),
            Game1.staminaRect.Bounds, StardewValley.Utility.getRedToGreenLerpColor(overcharge), 0f, Vector2.Zero, SpriteEffects.None,
            0.887f);
    }
}