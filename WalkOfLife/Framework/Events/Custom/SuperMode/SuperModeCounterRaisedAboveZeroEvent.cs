namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeCounterRaisedAboveZeroEventHandler();

	internal class SuperModeCounterRaisedAboveZeroEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeCounterRaisedAboveZero += OnSuperModeCounterRaisedAboveZero;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeCounterRaisedAboveZero -= OnSuperModeCounterRaisedAboveZero;
		}

		/// <summary>Raised when the SuperModeCounter is raised from zero to any value greater than zero.</summary>
		public void OnSuperModeCounterRaisedAboveZero()
		{
			// stop waiting for counter to return to zero and start waiting for it to fill up
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeCounterReturnedToZeroEvent));
			ModEntry.Subscriber.Subscribe(new SuperModeBarRenderingHudEvent(),
				new SuperModeBuffDisplayUpdateTickedEvent(), new SuperModeCounterFilledEvent());
		}
	}
}