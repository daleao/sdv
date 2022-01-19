namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal class StaticReturnedToTitleEvent : ReturnedToTitleEvent
{
    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object sender, ReturnedToTitleEventArgs e)
    {
        // disable events
        EventManager.DisableAllForLocalPlayer();

        // reset mod state
        ModEntry.State.Value = new();
    }
}