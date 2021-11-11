namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeCounterFilledEventHandler();

	internal class SuperModeCounterFilledEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeCounterFilled += OnSuperModeCounterFilled;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeCounterFilled -= OnSuperModeCounterFilled;
		}

		/// <summary>Raised when SuperModeCounter is set to the max value.</summary>
		public void OnSuperModeCounterFilled()
		{
			// stop waiting for counter to raise above zero and start waiting for it to return to zero
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeCounterRaisedAboveZeroEvent));
			ModEntry.Subscriber.Subscribe(new SuperModeBarShakeTimerUpdateTickedEvent(),
				new SuperModeCounterReturnedToZeroEvent(), new SuperModeEnabledEvent());
		}
	}
}