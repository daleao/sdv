using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeBarShakeTimerUpdateTickedEvent : UpdateTickedEvent
	{
		private const int TIME_BETWEEN_SHAKES = 126, SHAKE_DURATION = 15;

		private int _shakeTimer, _nextShake;

		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (!Game1.game1.IsActive || !Game1.shouldTimePass()) return;

			if (_shakeTimer > 0)
			{
				ModEntry.ShouldShakeSuperModeBar = true;
				--_shakeTimer;
			}
			else
			{
				ModEntry.ShouldShakeSuperModeBar = false;
			}

			--_nextShake;
			if (_nextShake > 0) return;

			_shakeTimer = SHAKE_DURATION;
			_nextShake = TIME_BETWEEN_SHAKES;
		}
	}
}