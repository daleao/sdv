namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Globalization;
using System.Linq;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using Content;
using Extensions;

#endregion using directives

internal class HostConservationismDayEndingEvent : DayEndingEvent
{
    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        EventManager.Enable(typeof(MailRequestedEvent));

        if (Game1.dayOfMonth != 28) return;

        foreach (var farmer in Game1.getAllFarmers().Where(f => f.HasProfession(Profession.Conservationist)))
        {
            var trashCollectedThisSeason =
                farmer.ReadDataAs<uint>(DataField.ConservationistTrashCollectedThisSeason);
            if (trashCollectedThisSeason <= 0) return;

            var taxBonusNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxLevel / 100f,
                    ModEntry.Config.TaxDeductionCeiling);
            farmer.WriteData(DataField.ConservationistActiveTaxBonusPct,
                taxBonusNextSeason.ToString(CultureInfo.InvariantCulture));
            if (taxBonusNextSeason > 0)
            {
                ModEntry.ModHelper.GameContent.InvalidateCache(PathUtilities.NormalizeAssetName("Data/mail"));
                farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
            }

            farmer.WriteData(DataField.ConservationistTrashCollectedThisSeason, "0");
        }
    }
}