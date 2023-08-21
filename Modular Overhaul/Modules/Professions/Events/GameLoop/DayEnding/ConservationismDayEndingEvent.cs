namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using System.Globalization;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ConservationismDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="ConservationismDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ConservationismDayEndingEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.player.HasProfession(Profession.Conservationist);

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        var player = Game1.player;
        if (!TaxesModule.ShouldEnable)
        {
            var taxBonus = player.Read<float>(DataKeys.ConservationistActiveTaxDeduction);
            if (taxBonus > 0f)
            {
                var amountSold = Game1.game1.GetTotalSoldByPlayer(player);
                if (amountSold >= 0)
                {
                    player.Money += (int)(amountSold * taxBonus);
                }
            }
        }

        if (Game1.dayOfMonth != 28)
        {
            return;
        }

        var trashCollectedThisSeason = (int)player.Read<float>(DataKeys.ConservationistTrashCollectedThisSeason);
        player.Write(DataKeys.ConservationistTrashCollectedThisSeason, "0");
        if (trashCollectedThisSeason <= 0)
        {
            player.Write(DataKeys.ConservationistActiveTaxDeduction, "0");
            return;
        }

        var taxBonusForNextSeason =
            // ReSharper disable once PossibleLossOfFraction
            Math.Min(
                trashCollectedThisSeason / ProfessionsModule.Config.TrashNeededPerTaxDeduction / 100f,
                ProfessionsModule.Config.ConservationistTaxDeductionCeiling);
        player.Write(
            DataKeys.ConservationistActiveTaxDeduction,
            taxBonusForNextSeason.ToString(CultureInfo.InvariantCulture));
        if (taxBonusForNextSeason <= 0 || TaxesModule.ShouldEnable)
        {
            return;
        }

        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
        player.mailForTomorrow.Add($"{Manifest.UniqueID}/ConservationistTaxNotice");
    }
}
