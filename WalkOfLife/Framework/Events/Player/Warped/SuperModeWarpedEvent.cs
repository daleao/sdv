using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeWarpedEvent : WarpedEvent
	{
		/// <inheritdoc />
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

			if (e.NewLocation.AnyOfType(typeof(MineShaft), typeof(Woods), typeof(SlimeHutch), typeof(VolcanoDungeon)))
			{
				ModEntry.Subscriber.Subscribe(new SuperModeBarRenderingHudEvent());
			}
			else
			{
				ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBarFadeOutUpdateTickedEvent),
					typeof(SuperModeBarShakeTimerUpdateTickedEvent), typeof(SuperModeBarRenderingHudEvent));

				ModEntry.SuperModeCounter = 0;
				ModEntry.SuperModeBarAlpha = 1f;
				ModEntry.ShouldShakeSuperModeBar = false;
			}
		}
	}
}