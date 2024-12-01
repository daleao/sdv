namespace DaLion.Harmonics.Framework.Events;

#region using directives

using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="HarmonicsAssetRequestedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ChordDayStartedEvent(EventManager? manager = null)
    : DayStartedEvent(manager ?? HarmonicsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnDayStartedImpl(object? sender, DayStartedEventArgs e)
    {
        State.ResonantChords.Values.ForEach(chord => chord.Resonate(Game1.player));
    }
}
