using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Globalization;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.AssetEditors;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop.DayEnding;

internal class ConservationistDayEndingEvent : DayEndingEvent
{
    /// <inheritdoc />
    public override void OnDayEnding(object sender, DayEndingEventArgs e)
    {
        if (!ModEntry.ModHelper.Content.AssetEditors.ContainsType(typeof(MailEditor)))
            ModEntry.ModHelper.Content.AssetEditors.Add(new MailEditor());

        if (Game1.dayOfMonth != 28) return;

        foreach (var farmer in Game1.getAllFarmers())
        {
            var trashCollectedThisSeason = ModData.ReadAs<uint>("TrashCollectedThisSeason", farmer);
            if (trashCollectedThisSeason <= 0) return;

            var taxBonusNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxLevel / 100f,
                    ModEntry.Config.TaxDeductionCeiling);
            ModData.Write("ActiveTaxBonusPercent",
                taxBonusNextSeason.ToString(CultureInfo.InvariantCulture), farmer);
            if (taxBonusNextSeason > 0)
            {
                ModEntry.ModHelper.Content.InvalidateCache(PathUtilities.NormalizeAssetName("Data/mail"));
                farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
            }

            ModData.Write("TrashCollectedThisSeason", "0");
        }
    }
}