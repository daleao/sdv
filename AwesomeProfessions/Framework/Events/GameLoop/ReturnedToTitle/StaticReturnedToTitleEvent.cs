using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.ReturnedToTitle;

[UsedImplicitly]
internal class StaticReturnedToTitleEvent : ReturnedToTitleEvent
{
    /// <inheritdoc />
    public override void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        // unsubscribe events
        ModEntry.Subscriber.UnsubscribeLocalPlayerEvents();

        // reset Super Mode
        if (ModEntry.State.Value.SuperModeIndex > 0) ModEntry.State.Value.SuperModeIndex = -1;
    }
}