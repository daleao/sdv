namespace DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PipedDismissOneSecondUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PipedDismissOneSecondUpdateTickedEvent(EventManager? manager = null)
    : OneSecondUpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    private int _counter = 0;

    /// <inheritdoc />
    protected override void OnEnabled()
    {
        this._counter = 0;
    }

    /// <inheritdoc />
    protected override void OnOneSecondUpdateTickedImpl(object? sender, OneSecondUpdateTickedEventArgs e)
    {
        if (this._counter++ < 3)
        {
            return;
        }

        Game1.player.DismissMinions();
        this.Disable();
    }
}
