namespace DaLion.Stardew.Prairie.Training.Framework.Events;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Wrapper for <see cref="IInputEvents.ButtonPressed"/> that can be hooked or unhooked.</summary>
[UsedImplicitly]
internal class ButtonPressedEvent : IEvent
{
    /// <inheritdoc />
    public void Hook()
    {
        ModEntry.ModHelper.Events.Input.ButtonPressed += OnButtonPressed;
        Log.D("[Prairie] Hooked ButtonPressed event.");
    }

    /// <inheritdoc />
    public void Unhook()
    {
        ModEntry.ModHelper.Events.Input.ButtonPressed -= OnButtonPressed;
        Log.D("[Prairie] Unhooked ButtonPressed event.");
    }

    /// <summary>Raised after the player pressed a keyboard, mouse, or controller button.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {
        if (ModEntry.IsPlayingAbigailGame && ModEntry.Config.DebugKey.JustPressed())
            Debug.AdvanceMap();
    }
}