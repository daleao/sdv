namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using System;
using System.Globalization;
using System.Linq;
using DaLion.Common.Events;
using DaLion.Common.Extensions.SMAPI;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Professions.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ConservationismDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="ConservationismDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ConservationismDayEndingEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        if (Game1.dayOfMonth != 28)
        {
            return;
        }

        foreach (var farmer in Game1.getAllFarmers().Where(f => f.HasProfession(Profession.Conservationist)))
        {
            var trashCollectedThisSeason =
                farmer.Read<uint>("ConservationistTrashCollectedThisSeason");
            farmer.Write("ConservationistTrashCollectedThisSeason", "0");
            if (trashCollectedThisSeason <= 0)
            {
                return;
            }

            var taxBonusForNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(
                    trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxBonusPct / 100f,
                    ModEntry.Config.ConservationistTaxBonusCeiling);
            farmer.Write(
                "ConservationistActiveTaxBonusPct",
                taxBonusForNextSeason.ToString(CultureInfo.InvariantCulture));
            if (taxBonusForNextSeason <= 0 || ModEntry.TaxesConfig is not null)
            {
                continue;
            }

            ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
            farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
        }
    }
}
