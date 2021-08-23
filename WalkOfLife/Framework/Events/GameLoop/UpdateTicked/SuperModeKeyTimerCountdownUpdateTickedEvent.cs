using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeKeyTimerCountdownUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.game1.IsActive && Game1.shouldTimePass()) --ModEntry.SuperModeKeyTimer;
		}
	}
}