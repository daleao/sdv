using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeActivationTimerUpdateTickedEvent : UpdateTickedEvent
	{
		private const int BASE_SUPERMODE_ACTIVATION_DELAY = 60;

		private int _superModeActivationTimer =
			(int) (BASE_SUPERMODE_ACTIVATION_DELAY * ModEntry.Config.SuperModeActivationDelay);

		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.game1.IsActive && Game1.shouldTimePass()) --_superModeActivationTimer;

			if (_superModeActivationTimer > 0) return;
			ModEntry.IsSuperModeActive = true;
			ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}