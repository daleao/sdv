using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Globalization;
using System.IO;
using TheLion.Stardew.Common.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events
{
	public class ConservationistDayEndingEvent : DayEndingEvent
	{
		/// <inheritdoc/>
		public override void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			if (!ModEntry.Content.AssetEditors.ContainsType(typeof(AssetEditors.FRSMailEditor)))
				ModEntry.Content.AssetEditors.Add(new AssetEditors.FRSMailEditor());

			uint trashCollectedThisSeason;
			if (Game1.dayOfMonth == 28 && (trashCollectedThisSeason = ModEntry.Data.ReadField<uint>("WaterTrashCollectedThisSeason")) > 0)
			{
				var taxBonusNextSeason = Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededForNextTaxLevel / 100f, ModEntry.Config.TaxBonusCeiling);
				ModEntry.Data.WriteField("ActiveTaxBonusPercent", taxBonusNextSeason.ToString(CultureInfo.InvariantCulture));
				if (taxBonusNextSeason > 0)
				{
					ModEntry.Content.InvalidateCache(Path.Combine("Data", "mail"));
					Game1.addMailForTomorrow("ConservationistTaxNotice");
				}
				ModEntry.Data.WriteField("WaterTrashCollectedThisSeason", "0");
			}
		}
	}
}