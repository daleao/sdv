using StardewModdingAPI.Events;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeWarpedEvent : WarpedEvent
	{
		/// <inheritdoc />
		public override void OnWarped(object sender, WarpedEventArgs e)
		{
			if (!e.IsLocalPlayer || e.NewLocation.GetType() == e.OldLocation.GetType()) return;

			if (e.NewLocation.IsCombatZone())
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