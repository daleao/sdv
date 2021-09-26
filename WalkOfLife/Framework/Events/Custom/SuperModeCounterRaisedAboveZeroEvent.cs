namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeCounterRaisedAboveZeroEventHandler();

	public class SuperModeCounterRaisedAboveZeroEvent : BaseEvent
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
			ModEntry.Subscriber.Subscribe(new SuperModeBarRenderingHudEvent(), new SuperModeBuffDisplayUpdateTickedEvent());
		}
	}
}
