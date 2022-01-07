using TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

namespace TheLion.Stardew.Professions.Framework.Events.Custom.SuperMode;

public delegate void SuperModeGaugeFilledEventHandler();

internal class SuperModeGaugeFilledEvent : BaseEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public override void Hook()
    {
        ModEntry.State.Value.SuperModeGaugeFilled += OnSuperModeGaugeFilled;
    }

    /// <summary>Unhook this event from the event listener.</summary>
    public override void Unhook()
    {
        ModEntry.State.Value.SuperModeGaugeFilled -= OnSuperModeGaugeFilled;
    }

    /// <summary>Raised when SuperModeGauge is set to the max value.</summary>
    public void OnSuperModeGaugeFilled()
    {
        // stop waiting for gauge to raise above zero and start waiting for it to return to zero
        ModEntry.Subscriber.UnsubscribeFrom(typeof(SuperModeGaugeRaisedAboveZeroEvent));
        ModEntry.Subscriber.SubscribeTo(new SuperModeBarShakeTimerUpdateTickedEvent(),
            new SuperModeGaugeReturnedToZeroEvent(), new SuperModeEnabledEvent());
    }
}