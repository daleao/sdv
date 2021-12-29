using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class ScavengerHuntUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
    {
        ModState.ScavengerHunt.Update(e.Ticks);
    }
}