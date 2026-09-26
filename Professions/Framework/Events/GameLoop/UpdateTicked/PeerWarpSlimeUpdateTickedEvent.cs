namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PeerWarpSlimeUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PeerWarpSlimeUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.Events)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsSplitScreen && Context.IsMainPlayer && PipedSlime.SlimesToBeWarped.Any();

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        if (e.Ticks % 10 != 0)
        {
            return;
        }

        var piped = PipedSlime.SlimesToBeWarped.Dequeue();
        piped.WarpToPiper();
    }
}
