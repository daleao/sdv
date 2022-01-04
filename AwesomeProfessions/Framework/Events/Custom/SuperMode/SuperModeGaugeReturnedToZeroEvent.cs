using StardewValley;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events.Custom.SuperMode;

public delegate void SuperModeGaugeReturnedToZeroEventHandler();

internal class SuperModeGaugeReturnedToZeroEvent : BaseEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public override void Hook()
    {
        ModEntry.State.Value.SuperModeGaugeReturnedToZero += OnSuperModeGaugeReturnedToZero;
    }

    /// <summary>Unhook this event from the event listener.</summary>
    public override void Unhook()
    {
        ModEntry.State.Value.SuperModeGaugeReturnedToZero -= OnSuperModeGaugeReturnedToZero;
    }

    /// <summary>Raised when SuperModeGauge is set to zero.</summary>
    public void OnSuperModeGaugeReturnedToZero()
    {
        if (!ModEntry.State.Value.IsSuperModeActive) return;
        ModEntry.State.Value.IsSuperModeActive = false;

        // stop waiting for gauge to fill up and start waiting for it to raise above zero
        ModEntry.Subscriber.Unsubscribe(typeof(SuperModeGaugeFilledEvent));
        ModEntry.Subscriber.Subscribe(new SuperModeGaugeRaisedAboveZeroEvent());
        if (!Game1.currentLocation.IsCombatZone())
            ModEntry.Subscriber.Subscribe(new SuperModeBarFadeOutUpdateTickedEvent());
    }
}