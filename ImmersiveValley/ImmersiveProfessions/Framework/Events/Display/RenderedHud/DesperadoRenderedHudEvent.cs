namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Ultimates;
using DaLion.Stardew.Professions.Framework.VirtualProperties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using Utility = StardewValley.Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class DesperadoRenderedHudEvent : RenderedHudEvent
{
    /// <summary>Initializes a new instance of the <see cref="DesperadoRenderedHudEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DesperadoRenderedHudEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnRenderedHudImpl(object? sender, RenderedHudEventArgs e)
    {
        var lastUser = Game1.player;
        if (lastUser.CurrentTool is not Slingshot slingshot || !lastUser.usingSlingshot ||
            lastUser.Get_Ultimate() is DeathBlossom { IsActive: true })
        {
            return;
        }

        var overcharge = slingshot.GetOvercharge(Game1.player);
        if (overcharge <= 0f)
        {
            return;
        }

        e.SpriteBatch.Draw(
            Game1.mouseCursors,
            Game1.GlobalToLocal(Game1.viewport, lastUser.Position + new Vector2(-48f, -160f)),
            new Rectangle(193, 1868, 47, 12),
            Color.White,
            0f,
            Vector2.Zero,
            Game1.pixelZoom,
            SpriteEffects.None,
            0.885f);

        e.SpriteBatch.Draw(
            Game1.staminaRect,
            new Rectangle(
                (int)Game1.GlobalToLocal(Game1.viewport, lastUser.Position).X - 36,
                (int)Game1.GlobalToLocal(Game1.viewport, lastUser.Position).Y - 148,
                (int)(164f * overcharge),
                25),
            Game1.staminaRect.Bounds,
            Utility.getRedToGreenLerpColor(overcharge),
            0f,
            Vector2.Zero,
            SpriteEffects.None,
            0.887f);
    }
}
