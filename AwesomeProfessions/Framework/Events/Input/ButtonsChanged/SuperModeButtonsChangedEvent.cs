using StardewModdingAPI;
using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

namespace TheLion.Stardew.Professions.Framework.Events.Input.ButtonsChanged;

internal class SuperModeButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    public override void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.SuperModeKey.JustPressed() && !ModEntry.State.Value.IsSuperModeActive &&
            ModEntry.State.Value.SuperModeGaugeValue >= ModEntry.State.Value.SuperModeGaugeMaxValue)
        {
            if (ModEntry.Config.HoldKeyToActivateSuperMode)
                ModEntry.Subscriber.SubscribeTo(new SuperModeActivationTimerUpdateTickedEvent());
            else
                ModEntry.State.Value.IsSuperModeActive = true;
        }
        else if (ModEntry.Config.SuperModeKey.GetState() == SButtonState.Released)
        {
            ModEntry.Subscriber.UnsubscribeFrom(typeof(SuperModeActivationTimerUpdateTickedEvent));
        }
    }
}