namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeRegisteredEventHandler();

	public class StaticSuperModeRegisteredEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeRegistered += OnSuperModeRegistered;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeRegistered -= OnSuperModeRegistered;
		}

		/// <summary>Raised when IsSuperModeActive is set to true.</summary>
		public void OnSuperModeRegistered()
		{
			if (ModEntry.SuperModeIndex >= 0) ModEntry.Subscriber.SubscribeSuperModeEvents();
			else ModEntry.Subscriber.UnsubscribeSuperModeEvents();
		}
	}
}
