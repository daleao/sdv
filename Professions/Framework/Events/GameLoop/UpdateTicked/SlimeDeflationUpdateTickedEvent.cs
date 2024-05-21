namespace DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SlimeDeflationUpdateTickedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class SlimeDeflationUpdateTickedEvent(EventManager? manager = null)
    : UpdateTickedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        var count = 0;
        foreach (var piped in State.AlliedSlimes)
        {
            if (piped is null || piped.PipeTimer > 0 || piped.Instance.Scale <= piped.OriginalScale)
            {
                continue;
            }

            piped.Deflate();
            count++;
        }

        if (count == 0)
        {
            this.Disable();
        }
    }
}
