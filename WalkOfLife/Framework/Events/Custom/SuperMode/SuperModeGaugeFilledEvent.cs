namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeGaugeFilledEventHandler();

	internal class SuperModeGaugeFilledEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModState.SuperModeGaugeFilled += OnSuperModeGaugeFilled;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModState.SuperModeGaugeFilled -= OnSuperModeGaugeFilled;
		}

		/// <summary>Raised when SuperModeGauge is set to the max value.</summary>
		public void OnSuperModeGaugeFilled()
		{
			// stop waiting for gauge to raise above zero and start waiting for it to return to zero
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeGaugeRaisedAboveZeroEvent));
			ModEntry.Subscriber.Subscribe(new SuperModeBarShakeTimerUpdateTickedEvent(),
				new SuperModeGaugeReturnedToZeroEvent(), new SuperModeEnabledEvent());
		}
	}
}