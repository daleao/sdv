namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeIndexChangedEventHandler();

	public class StaticSuperModeIndexChangedEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeIndexChanged += OnSuperModeIndexChanged;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeIndexChanged -= OnSuperModeIndexChanged;
		}

		/// <summary>Raised when IsSuperModeActive is set to true.</summary>
		public void OnSuperModeIndexChanged()
		{
			ModEntry.Subscriber.UnsubscribeSuperModeEvents();

			ModEntry.SuperModeCounter = 0;
			ModEntry.SuperModeBarOpacity = 1f;
			ModEntry.ShouldShakeSuperModeBar = false;

			if (ModEntry.SuperModeIndex > -1) ModEntry.Subscriber.SubscribeSuperModeEvents();

			ModEntry.Data.WriteField("SuperModeIndex", ModEntry.SuperModeIndex.ToString());
		}
	}
}
