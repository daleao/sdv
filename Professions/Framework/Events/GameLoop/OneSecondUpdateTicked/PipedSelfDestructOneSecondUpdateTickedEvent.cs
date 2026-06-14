namespace DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;

#region using directives

using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PipedSelfDestructOneSecondUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[ImplicitIgnore]
// Deprecated
internal sealed class PipedSelfDestructOneSecondUpdateTickedEvent(EventManager? manager = null)
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

        foreach (var (_, piped) in GreenSlime_Piped.Values)
        {
            if (piped.Source is PipedSlime.PipingSource.Summoned or PipedSlime.PipingSource.Charmed)
            {
                piped.Burst();
            }
        }

        this.Disable();
    }
}
