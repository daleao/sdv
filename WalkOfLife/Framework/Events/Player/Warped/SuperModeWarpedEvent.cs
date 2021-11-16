using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	internal class SuperModeWarpedEvent : WarpedEvent
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

				ModState.SuperModeGaugeValue = 0;
				ModState.SuperModeGaugeAlpha = 1f;
				ModState.ShouldShakeSuperModeGauge = false;

				var buffID = ModEntry.Manifest.UniqueID.Hash() + ModState.SuperModeIndex + 4;
				var buff = Game1.buffsDisplay.otherBuffs.FirstOrDefault(p => p.which == buffID);
				if (buff is null) return;

				Game1.buffsDisplay.otherBuffs.Remove(buff);
				Game1.player.stopGlowing();
			}
		}
	}
}