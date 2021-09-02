using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class SuperModeCountdownUpdateTickedEvent : UpdateTickedEvent
	{
		/// <inheritdoc/>
		public override void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.game1.IsActive && Game1.shouldTimePass() && e.IsMultipleOf(ModEntry.Config.SuperModeDrainFactor)) --ModEntry.SuperModeCounter;
			if (ModEntry.SuperModeCounter <= 0) ModEntry.Subscriber.Unsubscribe(GetType());
		}
	}
}