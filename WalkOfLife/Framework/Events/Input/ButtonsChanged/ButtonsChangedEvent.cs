using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal abstract class ButtonsChangedEvent : BaseEvent
{
    /// <inheritdoc />
    public override void Hook()
    {
        ModEntry.ModHelper.Events.Input.ButtonsChanged += OnButtonsChanged;
    }

    /// <inheritdoc />
    public override void Unhook()
    {
        ModEntry.ModHelper.Events.Input.ButtonsChanged -= OnButtonsChanged;
    }

    /// <summary>Raised after the player pressed/released any buttons on the keyboard, mouse, or controller.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    public abstract void OnButtonsChanged(object sender, ButtonsChangedEventArgs e);
}