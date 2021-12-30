using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeButtonsChangedEvent : ButtonsChangedEvent
{
    /// <inheritdoc />
    public override void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        if (ModEntry.Config.SuperModeKey.JustPressed() && !ModState.IsSuperModeActive &&
            ModState.SuperModeGaugeValue >= ModState.SuperModeGaugeMaxValue)
        {
            if (ModEntry.Config.HoldKeyToActivateSuperMode)
                ModEntry.Subscriber.Subscribe(new SuperModeActivationTimerUpdateTickedEvent());
            else
                ModState.IsSuperModeActive = true;
        }
        else if (ModEntry.Config.SuperModeKey.GetState() == SButtonState.Released)
        {
            ModEntry.Subscriber.Unsubscribe(typeof(SuperModeActivationTimerUpdateTickedEvent));
        }
    }
}