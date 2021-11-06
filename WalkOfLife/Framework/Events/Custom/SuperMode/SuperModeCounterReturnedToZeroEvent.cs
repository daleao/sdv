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

			if (!Game1.currentLocation.IsCombatZone())
				ModEntry.Subscriber.Subscribe(new SuperModeBarFadeOutUpdateTickedEvent());
		}
	}
}