using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

[UsedImplicitly]
internal class StaticReturnedToTitleEvent : ReturnedToTitleEvent
{
    /// <inheritdoc />
    public override void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        // release mod data
        ModEntry.Data.Unload();

        // unsubscribe events
        ModEntry.Subscriber.UnsubscribeLocalPlayerEvents();

        // reset Super Mode
        if (ModState.SuperModeIndex > 0) ModState.SuperModeIndex = -1;
    }
}