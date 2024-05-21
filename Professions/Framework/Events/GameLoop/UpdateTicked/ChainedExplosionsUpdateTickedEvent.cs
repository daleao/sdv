namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ChainedExplosionsUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ChainedExplosionsUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => State.ChainedExplosions.Count > 0;

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (!e.IsMultipleOf(4))
        {
            return;
        }

        for (var i = State.ChainedExplosions.Count - 1; i >= 0; i--)
        {
            if (State.ChainedExplosions[i].Update())
            {
                State.ChainedExplosions.RemoveAt(i);
            }
        }
    }
}
