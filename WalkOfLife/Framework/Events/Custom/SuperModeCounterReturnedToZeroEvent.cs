using StardewValley;
using StardewValley.Locations;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public delegate void SuperModeCounterReturnedToZeroEventHandler();

	public class SuperModeCounterReturnedToZeroEvent : BaseEvent
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
			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBuffsDisplayUpdateTickedEvent));

			if (!ModEntry.IsSuperModeActive) return;
			ModEntry.IsSuperModeActive = false;

			if (!Game1.currentLocation.AnyOfType(typeof(MineShaft), typeof(Woods), typeof(SlimeHutch), typeof(VolcanoDungeon)))
				ModEntry.Subscriber.Subscribe(new SuperModeBarFadeOutUpdateTickedEvent());
		}
	}
}
