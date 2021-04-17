using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using TheLion.Common;
using SObject = StardewValley.Object;

namespace TheLion.AwesomeProfessions
{
	internal class ArtisanDayEndingEvent : DayEndingEvent
	{
		/// <inheritdoc/>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			if (!AwesomeProfessions.Content.AssetEditors.ContainsType(typeof(SASMailEditor)))
				AwesomeProfessions.Content.AssetEditors.Add(new SASMailEditor());

			// get Artisan fame points for the day
			foreach (var item in Game1.getFarm().getShippingBin(Game1.player).Where(item => item is SObject { Category: SObject.artisanGoodsCategory }))
			{
				var obj = (SObject)item;
				AwesomeProfessions.Data.IncrementField($"{AwesomeProfessions.UniqueID}/ArtisanPointsAccrued", (long)(obj.Price * obj.Quality * obj.Stack));
			}

			if (Game1.dayOfMonth != 22) return;

			// try for level up
			var artisanPointsAccrued = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ArtisanPointsAccrued", long.Parse);
			var currentLevel = (int)Math.Sqrt(artisanPointsAccrued / (5E4 * Math.Pow(1.1, AwesomeProfessions.Config.ArtisanLevelUpDifficulty)) - 4) + 1;
			if (currentLevel <= AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/ArtisanAwardLevel", int.Parse)) return;
			
			AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/ArtisanAwardLevel", currentLevel.ToString());
			Game1.addMailForTomorrow($"{AwesomeProfessions.UniqueID}/ArtisanAwardNotice{currentLevel}");
			if (currentLevel >= 5) AwesomeProfessions.EventManager.Unsubscribe(this.GetType());
		}
	}
}