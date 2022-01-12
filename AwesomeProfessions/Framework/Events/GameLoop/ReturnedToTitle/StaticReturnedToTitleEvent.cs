using JetBrains.Annotations;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop;

[UsedImplicitly]
internal class StaticReturnedToTitleEvent : ReturnedToTitleEvent
{
    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object sender, ReturnedToTitleEventArgs e)
    {
        // disable events
        ModEntry.EventManager.DisableAllForLocalPlayer();

        // reset mod state
        ModEntry.State.Value = new();
    }
}