namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IGameLoopEvents.ReturnedToTitle"/> allowing dynamic enabling / disabling.</summary>
internal abstract class ReturnedToTitleEvent : BaseEvent
{
    /// <inheritdoc cref="IGameLoopEvents.ReturnedToTitle"/>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    public void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
    {
        if (enabled.Value) OnReturnedToTitleImpl(sender, e);
    }

    /// <inheritdoc cref="OnReturnedToTitle" />
    protected abstract void OnReturnedToTitleImpl(object sender, ReturnedToTitleEventArgs e);
}