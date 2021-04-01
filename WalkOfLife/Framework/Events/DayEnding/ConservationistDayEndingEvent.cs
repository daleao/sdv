using StardewModdingAPI.Events;
using StardewValley;
using System.IO;
using TheLion.Common;

namespace TheLion.AwesomeProfessions
{
	internal class ConservationistDayEndingEvent : DayEndingEvent
	{
		/// <inheritdoc/>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			uint trashCollectedThisSeason;
			if (Game1.dayOfMonth == 28 && (trashCollectedThisSeason = AwesomeProfessions.Data.ReadField($"{AwesomeProfessions.UniqueID}/WaterTrashCollectedThisSeason", uint.Parse)) > 0)
			{
				float taxBonusNextSeason = trashCollectedThisSeason / AwesomeProfessions.Config.TrashNeededForNextTaxLevel / 100f;
				AwesomeProfessions.Data.WriteField($"{AwesomeProfessions.UniqueID}/ActiveTaxBonus", taxBonusNextSeason.ToString());
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