using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using TheLion.Stardew.Common.Extensions;
using SObject = StardewValley.Object;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ArtisanDayEndingEvent : DayEndingEvent
	{
		/// <inheritdoc/>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			if (!ModEntry.Content.AssetEditors.ContainsType(typeof(AssetEditors.SASMailEditor)))
				ModEntry.Content.AssetEditors.Add(new AssetEditors.SASMailEditor());

			// get Artisan fame points for the day
			foreach (var item in Game1.getFarm().getShippingBin(Game1.player).Where(item => item is SObject { Category: SObject.artisanGoodsCategory }))
			{
				var obj = (SObject)item;
				ModEntry.Data.IncrementField("ArtisanPointsAccrued", (uint)(obj.Price * Math.Pow(obj.Quality, 2) * obj.Stack / 1000));
			}

			if (Game1.dayOfMonth != 28) return;

			// try for level up
			var artisanPointsAccrued = ModEntry.Data.ReadField<uint>("ArtisanPointsAccrued");
			var newLevel = artisanPointsAccrued switch
			{
				>= 10000 => 5,
				>= 5000 => 4,
				>= 2500 => 3,
				>= 1000 => 2,
				>= 500 => 1,
				< 500 => 0
			};
			if (newLevel <= ModEntry.Data.ReadField<uint>("ArtisanAwardLevel")) return;

			ModEntry.Data.WriteField("ArtisanAwardLevel", newLevel.ToString());
			Game1.addMailForTomorrow($"{ModEntry.UniqueID}/ArtisanAwardNotice{newLevel}");
			if (newLevel >= 5) ModEntry.Subscriber.Unsubscribe(this.GetType());
		}
	}
}