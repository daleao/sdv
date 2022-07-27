namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Events;
using Common.ModData;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Globalization;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ConservationismDayEndingEvent : DayEndingEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ConservationismDayEndingEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        if (Game1.dayOfMonth != 28) return;

        foreach (var farmer in Game1.getAllFarmers().Where(f => f.HasProfession(Profession.Conservationist)))
        {
            var trashCollectedThisSeason =
                ModDataIO.Read<uint>(farmer, "ConservationistTrashCollectedThisSeason");
            if (trashCollectedThisSeason <= 0) return;

            var taxBonusForNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxBonusPct / 100f,
                    ModEntry.Config.ConservationistTaxBonusCeiling);
            ModDataIO.Write(farmer, "ConservationistActiveTaxBonusPct",
                taxBonusForNextSeason.ToString(CultureInfo.InvariantCulture));
            if (taxBonusForNextSeason > 0 && ModEntry.TaxesConfig is null)
            {
                ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
            }

            ModDataIO.Write(farmer, "ConservationistTrashCollectedThisSeason", "0");
        }
    }
}