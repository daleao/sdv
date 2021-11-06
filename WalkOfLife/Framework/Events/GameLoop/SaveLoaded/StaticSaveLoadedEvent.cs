using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events
{
	[UsedImplicitly]
	internal class StaticSaveLoadedEvent : SaveLoadedEvent
	{
		/// <inheritdoc />
		public override void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			ModEntry.Log("Save loaded.", LogLevel.Trace);

			// load persisted mod data
			ModEntry.Data.Load();

			// subcribe player's profession events
			ModEntry.Subscriber.SubscribeEventsForLocalPlayer();

			// set super mode
			ModEntry.Log("Loading persisted Super Mode index.", LogLevel.Trace);
			ModEntry.SuperModeIndex = ModEntry.Data.ReadField<int>("SuperModeIndex");

			// check for mismatch between saved data and player professions
			switch (ModEntry.SuperModeIndex)
			{
				case < 0 when Game1.player.professions.Any(p => p is >= 26 and < 30):
					ModEntry.SuperModeIndex = Game1.player.professions.First(p => p is >= 26 and < 30);
					break;

				case > 0 when !Game1.player.professions.Any(p => p is >= 26 and < 30):
					ModEntry.SuperModeIndex = -1;
					break;
			}
		}
	}
}