using StardewModdingAPI.Events;

namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

internal class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.ProspectorHunt.Update(e.Ticks);
    }
}