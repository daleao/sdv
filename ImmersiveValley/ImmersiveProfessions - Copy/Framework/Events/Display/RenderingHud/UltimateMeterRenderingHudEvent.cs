namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateMeterRenderingHudEvent : RenderingHudEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateMeterRenderingHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object? sender, RenderingHudEventArgs e)
    {
        if (ModEntry.Player.RegisteredUltimate is null)
        {
            Unhook();
            return;
        }

        if (!Game1.eventUp) ModEntry.Player.RegisteredUltimate.Hud.Draw(e.SpriteBatch);
    }
}