using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop;

internal class SuperModeBuffDisplayUpdateTickedEvent : UpdateTickedEvent
{
    /// <inheritdoc />
    protected override void OnUpdateTickedImpl(object sender, UpdateTickedEventArgs e)
    {
        ModEntry.State.Value.SuperMode.AddBuff();
    }
}