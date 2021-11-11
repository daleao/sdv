using StardewValley;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeCounterReturnedToZeroEventHandler();

	internal class SuperModeCounterReturnedToZeroEvent : BaseEvent
	{
		/// <summary>Hook this event to the event listener.</summary>
		public override void Hook()
		{
			ModEntry.SuperModeCounterReturnedToZero += OnSuperModeCounterReturnedToZero;
		}

		/// <summary>Unhook this event from the event listener.</summary>
		public override void Unhook()
		{
			ModEntry.SuperModeCounterReturnedToZero -= OnSuperModeCounterReturnedToZero;
		}

		/// <summary>Raised when SuperModeCounter is set to zero.</summary>
		public void OnSuperModeCounterReturnedToZero()
		{
			if (!ModEntry.IsSuperModeActive) return;
			ModEntry.IsSuperModeActive = false;

			// stop waiting for counter to fill up and start waiting for it to raise above zero
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeCounterFilledEvent));
			ModEntry.Subscriber.Subscribe(new SuperModeCounterRaisedAboveZeroEvent());
			if (!Game1.currentLocation.IsCombatZone())
				ModEntry.Subscriber.Subscribe(new SuperModeBarFadeOutUpdateTickedEvent());
		}
	}
}