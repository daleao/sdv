namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class StaticReturnedToTitleEvent : ReturnedToTitleEvent
{
    /// <inheritdoc />
    protected override void OnReturnedToTitleImpl(object sender, ReturnedToTitleEventArgs e)
    {
        // unhook events
        ModEntry.EventManager.UnhookFromLocalPlayer();

        // reset mod state
        ModEntry.PlayerState = new();
    }
}