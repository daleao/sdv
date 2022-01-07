using TheLion.Stardew.Professions.Framework.Events.Display.RenderingHud;
using TheLion.Stardew.Professions.Framework.Events.GameLoop.UpdateTicked;

namespace TheLion.Stardew.Professions.Framework.Events.Custom.SuperMode;

public delegate void SuperModeGaugeRaisedAboveZeroEventHandler();

internal class SuperModeGaugeRaisedAboveZeroEvent : BaseEvent
{
    /// <summary>Hook this event to the event listener.</summary>
    public override void Hook()
    {
        ModEntry.State.Value.SuperModeGaugeRaisedAboveZero += OnSuperModeGaugeRaisedAboveZero;
    }

    /// <summary>Unhook this event from the event listener.</summary>
    public override void Unhook()
    {
        ModEntry.State.Value.SuperModeGaugeRaisedAboveZero -= OnSuperModeGaugeRaisedAboveZero;
    }

    /// <summary>Raised when SuperModeGauge is raised from zero to any value greater than zero.</summary>
    public void OnSuperModeGaugeRaisedAboveZero()
    {
        // stop waiting for gauge to return to zero and start waiting for it to fill up
        ModEntry.Subscriber.UnsubscribeFrom(typeof(SuperModeGaugeReturnedToZeroEvent));
        ModEntry.Subscriber.SubscribeTo(new SuperModeBarRenderingHudEvent(),
            new SuperModeBuffDisplayUpdateTickedEvent(), new SuperModeGaugeFilledEvent());
    }
}