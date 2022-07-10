namespace DaLion.Stardew.Professions.Framework.Events.GameLoop;

#region using directives

using Common.Data;
using Common.Events;
using Extensions;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Globalization;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class HostConservationismDayEndingEvent : DayEndingEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal HostConservationismDayEndingEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        if (Game1.dayOfMonth != 28) return;

        foreach (var farmer in Game1.getAllFarmers().Where(f => f.HasProfession(Profession.Conservationist)))
        {
            var trashCollectedThisSeason =
                ModDataIO.ReadFrom<uint>(farmer, "ConservationistTrashCollectedThisSeason");
            if (trashCollectedThisSeason <= 0) return;

            var taxBonusForNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(trashCollectedThisSeason / ModEntry.Config.TrashNeededPerTaxBonusPct / 100f,
                    ModEntry.Config.ConservationistTaxBonusCeiling);
            ModDataIO.WriteTo(farmer, "ConservationistActiveTaxBonusPct",
                taxBonusForNextSeason.ToString(CultureInfo.InvariantCulture));
            if (taxBonusForNextSeason > 0 && ModEntry.TaxesConfig is null)
            {
                ModEntry.ModHelper.GameContent.InvalidateCache("Data/mail");
                farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
            }

            ModDataIO.WriteTo(farmer, "ConservationistTrashCollectedThisSeason", "0");
        }
    }
}