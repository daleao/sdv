using System;
using System.Globalization;
using System.Linq;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.AssetEditors;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Events.GameLoop;

internal class GlobalConservationistDayEndingEvent : DayEndingEvent
{
    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        if (!ModEntry.ModHelper.Content.AssetEditors.ContainsType(typeof(MailEditor)))
            ModEntry.ModHelper.Content.AssetEditors.Add(new MailEditor());

        if (Game1.dayOfMonth != 28) return;

        foreach (var farmer in Game1.getAllFarmers().Where(f => f.HasProfession("Conservationist")))
        {
            var trashCollectedThisSeason =
                ModData.ReadAs<uint>(DataField.ConservationistTrashCollectedThisSeason, farmer);
            if (trashCollectedThisSeason <= 0) return;

            var taxBonusNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxLevel / 100f,
                    ModEntry.Config.TaxDeductionCeiling);
            ModData.Write(DataField.ConservationistActiveTaxBonusPct,
                taxBonusNextSeason.ToString(CultureInfo.InvariantCulture), farmer);
            if (taxBonusNextSeason > 0)
            {
                ModEntry.ModHelper.Content.InvalidateCache(PathUtilities.NormalizeAssetName("Data/mail"));
                farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
            }

            ModData.Write(DataField.ConservationistTrashCollectedThisSeason, "0", farmer);
        }
    }
}