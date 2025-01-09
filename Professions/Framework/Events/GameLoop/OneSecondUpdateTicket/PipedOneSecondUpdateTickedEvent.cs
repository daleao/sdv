namespace DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicket;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PipedOneSecondUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PipedOneSecondUpdateTickedEvent(EventManager? manager = null)
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
        if (this._counter++ <= 2)
        {
            return;
        }

        if (State.AlliedSlimes[0] is { } piped1)
        {
            piped1.DropItems();
            piped1.Burst();
            State.AlliedSlimes[0] = null;
        }

        if (State.AlliedSlimes[1] is { } piped2)
        {
            piped2.DropItems();
            piped2.Burst();
            State.AlliedSlimes[1] = null;
        }

        this.Disable();
    }
}
