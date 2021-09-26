namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeCounterFilledEventHandler();

	public class SuperModeCounterFilledEvent : BaseEvent
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
			ModEntry.Subscriber.Subscribe(new SuperModeBarShakeTimerUpdateTickedEvent());

		}
	}
}
