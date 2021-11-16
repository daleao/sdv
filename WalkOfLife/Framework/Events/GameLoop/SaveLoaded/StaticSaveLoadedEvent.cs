using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.AssetEditors;
using TheLion.Stardew.Professions.Framework.Extensions;

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

			// set Super Mode
			ModEntry.Log("Loading persisted Super Mode index.", LogLevel.Trace);
			ModState.SuperModeIndex = ModEntry.Data.Read<int>("SuperModeIndex");

			// check for mismatch between saved data and player professions
			switch (ModState.SuperModeIndex)
			{
				case < 0 when Game1.player.professions.Any(p => p is >= 26 and < 30):
					ModState.SuperModeIndex = Game1.player.professions.First(p => p is >= 26 and < 30);
					break;

				case > 0 when !Game1.player.professions.Any(p => p is >= 26 and < 30):
					ModState.SuperModeIndex = -1;
					break;
			}

			// check for prestige achievements
			if (!Game1.player.HasAllProfessions()) return;

			string name = ModEntry.ModHelper.Translation.Get("prestige.achievement.name." + (Game1.player.IsMale ? "male" : "female"));
			if (Game1.player.achievements.Contains(name.Hash()) && !ModEntry.ModHelper.Content.AssetEditors.ContainsType(typeof(AchivementsEditor)))
				ModEntry.ModHelper.Content.AssetEditors.Add(new AchivementsEditor());
			else
				ModEntry.Subscriber.Subscribe(new AchievementUnlockedDayStartedEvent());
		}
	}
}