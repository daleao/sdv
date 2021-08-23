using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SlimeContactTimerCountdownUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (ModEntry.SlimeContactTimer > 0 && Game1.shouldTimePass()) --ModEntry.SlimeContactTimer;
		}
	}
}