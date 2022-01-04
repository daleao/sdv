using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

internal class ScavengerHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.ScavengerHunt.Update(e.Ticks);
    }
}