namespace DaLion.Stardew.Professions.Framework.Events.Input;

#region using directives

using StardewModdingAPI.Events;

#endregion using directives

internal abstract class ButtonsChangedEvent : BaseEvent
{
    /// <summary>Raised after the player pressed/released any buttons on the keyboard, mouse, or controller.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (enabled.Value) OnButtonsChangedImpl(sender, e);
    }

    /// <inheritdoc cref="OnButtonsChanged" />
    protected abstract void OnButtonsChangedImpl(object sender, ButtonsChangedEventArgs e);
}