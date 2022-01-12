using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop;

internal class IndicatorUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.Indicator.Update(e.Ticks);
    }
}