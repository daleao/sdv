namespace DaLion.Stardew.Rings.Framework.Events;

#region using directives

using DaLion.Common.Events;
using DaLion.Stardew.Rings.Framework.Resonance;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class ResonanceUpdateTickedEvent : UpdateTickedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ResonanceUpdateTickedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ResonanceUpdateTickedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object? sender, UpdateTickedEventArgs e)
    {
        Chord.Vibrate();
    }
}
