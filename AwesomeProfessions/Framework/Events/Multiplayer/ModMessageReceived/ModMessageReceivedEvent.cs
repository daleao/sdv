using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.Multiplayer;

internal abstract class ModMessageReceivedEvent : BaseEvent
{
    /// <summary>Raised after a mod message is received over the network.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        if (enabled.Value) OnModMessageReceivedImpl(sender, e);
    }

    /// <inheritdoc cref="OnModMessageReceived" />
    protected abstract void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e);
}