using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events;

public delegate void SuperModeGaugeReturnedToZeroEventHandler();

internal class SuperModeGaugeReturnedToZeroEvent : BaseEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public override void Hook()
    {
        ModState.SuperModeGaugeReturnedToZero += OnSuperModeGaugeReturnedToZero;
    }

    /// <summary>Unhook this event from the event listener.</summary>
    public override void Unhook()
    {
        ModState.SuperModeGaugeReturnedToZero -= OnSuperModeGaugeReturnedToZero;
    }

    /// <summary>Raised when SuperModeGauge is set to zero.</summary>
    public void OnSuperModeGaugeReturnedToZero()
    {
        if (!ModState.IsSuperModeActive) return;
        ModState.IsSuperModeActive = false;

        // stop waiting for gauge to fill up and start waiting for it to raise above zero
        ModEntry.Subscriber.Unsubscribe(typeof(SuperModeGaugeFilledEvent));
        ModEntry.Subscriber.Subscribe(new SuperModeGaugeRaisedAboveZeroEvent());
        if (!Game1.currentLocation.IsCombatZone())
            ModEntry.Subscriber.Subscribe(new SuperModeBarFadeOutUpdateTickedEvent());
    }
}