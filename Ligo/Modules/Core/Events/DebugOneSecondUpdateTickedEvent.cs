namespace DaLion.Ligo.Modules.Core.Events;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugOneSecondUpdateTickedEvent : OneSecondUpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebugOneSecondUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DebugOneSecondUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        // show FPS counter
        Globals.FpsCounter?.Update(Game1.currentGameTime);
    }
}
