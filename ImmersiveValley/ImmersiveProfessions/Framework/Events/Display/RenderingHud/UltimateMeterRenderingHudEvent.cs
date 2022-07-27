namespace DaLion.Stardew.Professions.Framework.Events.Display;

#region using directives

using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using Ultimates;
using VirtualProperties;

#endregion using directives

[UsedImplicitly, UltimateEvent]
internal sealed class UltimateMeterRenderingHudEvent : RenderingHudEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateMeterRenderingHudEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnRenderingHudImpl(object? sender, RenderingHudEventArgs e)
    {
        var ultimate = Game1.player.get_Ultimate();
        if (ultimate is null)
        {
            Disable();
            return;
        }

        if (!Game1.eventUp) ultimate.Hud.Draw(e.SpriteBatch);
    }
}