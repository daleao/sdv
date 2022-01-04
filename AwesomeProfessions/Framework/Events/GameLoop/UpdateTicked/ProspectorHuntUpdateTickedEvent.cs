using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal class ProspectorHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.ProspectorHunt.Update(e.Ticks);
    }
}