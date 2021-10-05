using StardewModdingAPI.Events;
using System;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBarFadeOutUpdateTickedEvent : UpdateTickedEvent
	{
		private const int FADE_OUT_DELAY = 120, FADE_OUT_DURATION = 30;

		private int _fadeOutTimer = FADE_OUT_DELAY + FADE_OUT_DURATION;

		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			--_fadeOutTimer;
			if (_fadeOutTimer >= FADE_OUT_DURATION) return;

			var ratio = (float) _fadeOutTimer / FADE_OUT_DURATION;
			ModEntry.SuperModeBarAlpha = (float) (-1.0 / (1.0 + Math.Exp(12.0 * ratio - 6.0)) + 1.0);

			if (_fadeOutTimer > 0) return;

			ModEntry.Subscriber.Unsubscribe(typeof(SuperModeBarRenderingHudEvent), GetType());
			ModEntry.SuperModeBarAlpha = 1f;
		}
	}
}