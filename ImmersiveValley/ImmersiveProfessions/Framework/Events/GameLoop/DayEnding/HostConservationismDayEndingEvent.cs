namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Data;
using Common.Events;
using Content;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class HostConservationismDayEndingEvent : DayEndingEvent
{
    /// <inheritdoc />
    protected override void OnDayEndingImpl(object sender, DayEndingEventArgs e)
    {
        if (Game1.dayOfMonth != 28) return;

        foreach (var farmer in Game1.getAllFarmers().Where(f => f.HasProfession(Profession.Conservationist)))
        {
            var trashCollectedThisSeason =
                ModDataIO.ReadDataAs<uint>(farmer, ModData.ConservationistTrashCollectedThisSeason.ToString());
            if (trashCollectedThisSeason <= 0) return;

            var taxBonusForNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxBonusPct / 100f,
                    ModEntry.Config.ConservationistTaxBonusCeiling);
            ModDataIO.WriteData(farmer, ModData.ConservationistActiveTaxBonusPct.ToString(),
                taxBonusForNextSeason.ToString(CultureInfo.InvariantCulture));
            if (taxBonusForNextSeason > 0 && ModEntry.TaxesConfig is null)
            {
                ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
            }

            ModDataIO.WriteData(farmer, ModData.ConservationistTrashCollectedThisSeason.ToString(), "0");
        }
    }
}