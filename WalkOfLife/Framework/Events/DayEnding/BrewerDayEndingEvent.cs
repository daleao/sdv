using StardewModdingAPI.Events;
using StardewValley;
using System.Linq;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class BrewerDayEndingEvent : DayEndingEvent
	{
		private const uint _AwardLevelMax = 5;

		/// <inheritdoc/>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			if (!AwesomeProfessions.Content.AssetEditors.ContainsType(typeof(SBAMailEditor)))
				AwesomeProfessions.Content.AssetEditors.Add(new SBAMailEditor());

			// get Brewer fame points for the day
			foreach (SObject obj in Game1.getFarm().getShippingBin(Game1.player).Where(item => Utility.IsBeverage(item as SObject)))
			{
				if (obj.ParentSheetIndex.AnyOf(350, 614)) continue;

				float basePoints = obj.ParentSheetIndex switch
				{
					303 => 1.2f,
					346 => 1f,
					348 => 2f,
					459 => 1f,
					_ => 0
				};

				float multiplier = obj.Quality switch
				{
					SObject.bestQuality => 3f,
					SObject.highQuality => 1f,
					SObject.medQuality => 0f,
					SObject.lowQuality => -1f,
					_ => 0
				};

				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/BrewerFameAccrued", basePoints * multiplier * obj.Stack);
			}

			// check if should level up
			float brewerFameAccrued;
			if (Game1.dayOfMonth == 7 && (brewerFameAccrued = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/BrewerFameAccrued", float.Parse)) > 0)
			{
				float fameNeededForNextLevel = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/FameNeededForNextAwardLevel", float.Parse);
				if (brewerFameAccrued >= fameNeededForNextLevel)
				{
					AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/BrewerAwardLevel", amount: 1);
					AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/BrewerFameAccrued", (brewerFameAccrued - fameNeededForNextLevel).ToString());
					uint currentLevel = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/BrewerAwardLevel", uint.Parse);
					fameNeededForNextLevel = AwesomeProfessions.Config.BrewerLevelUpDifficulty * (10 * currentLevel * currentLevel - 20 * currentLevel + 50);
					AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/FameNeededForNextAwardLevel", fameNeededForNextLevel.ToString());
					Game1.addMailForTomorrow($"{AwesomeProfessions.UniqueID}/BrewerAwardNotice{currentLevel}");
				}
			}
		}
	}
}