using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Globalization;
using System.IO;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class ConservationistDayEndingEvent : DayEndingEvent
	{
		/// <inheritdoc/>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			if (!AwesomeProfessions.Content.AssetEditors.ContainsType(typeof(FRSMailEditor)))
				AwesomeProfessions.Content.AssetEditors.Add(new FRSMailEditor());

			uint trashCollectedThisSeason;
			if (Game1.dayOfMonth == 28 && (trashCollectedThisSeason = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/WaterTrashCollectedThisSeason", uint.Parse)) > 0)
			{
				var taxBonusNextSeason = Math.Min(trashCollectedThisSeason / AwesomeProfessions.Config.TrashNeededForNextTaxLevel / 100f, 0.37f);
				AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/ActiveTaxBonusPercent", taxBonusNextSeason.ToString(CultureInfo.InvariantCulture));
				if (taxBonusNextSeason > 0)
				{
					AwesomeProfessions.Content.InvalidateCache(Path.Combine("Data", "mail"));
					Game1.addMailForTomorrow($"{AwesomeProfessions.UniqueID}/ConservationistTaxNotice");
				}
				AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/WaterTrashCollectedThisSeason", "0");
			}
		}
	}
}