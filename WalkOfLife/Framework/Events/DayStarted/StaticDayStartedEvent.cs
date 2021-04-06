using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace TheLion.AwesomeProfessions
{
	internal class StaticDayStartedEvent : DayStartedEvent
	{
		/// <inheritdoc/>
		public override void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			AwesomeProfessions.EventManager.SubscribeMissingEvents();
			AwesomeProfessions.EventManager.CleanUpRogueEvents();
			LevelUpMenu.RevalidateHealth(Game1.player);
		}
	}
}