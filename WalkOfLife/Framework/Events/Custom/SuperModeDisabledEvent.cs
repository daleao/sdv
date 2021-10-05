namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeDisabledEventHandler();

	public class SuperModeDisabledEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeDisabled += OnSuperModeDisabled;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeDisabled -= OnSuperModeDisabled;
		}

		/// <summary>Raised when IsSuperModeActive is set to false.</summary>
		public void OnSuperModeDisabled()
		{
			// remove countdown and fade out overlay
			ModEntry.Subscriber.Subscribe(new SuperModeOverlayFadeOutUpdateTickedEvent());
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeCountdownUpdateTickedEvent));

			// notify peers
			ModEntry.ModHelper.Multiplayer.SendMessage(ModEntry.SuperModeIndex, "SuperModeDectivated",
				new[] {ModEntry.UniqueID});

			// remove permanent effects
			if (ModEntry.SuperModeIndex == Util.Professions.IndexOf("Piper"))
				ModEntry.Subscriber.Subscribe(new SlimeDeflationUpdateTickedEvent());
		}
	}
}