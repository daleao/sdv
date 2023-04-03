namespace DaLion.Overhaul.Modules.Professions.Events.GameLoop;

#region using directives

using System.Globalization;
using System.Linq;
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
            var taxBonus = player.Read<float>(DataKeys.ConservationistActiveTaxBonusPct);
            if (taxBonus > 0f)
            {
                var amountSold = Game1.getFarm().getShippingBin(player).Sum(item =>
                    item is SObject obj ? obj.sellToStorePrice() * obj.Stack : item.salePrice() / 2);
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

        var trashCollectedThisSeason = player.Read<uint>(DataKeys.ConservationistTrashCollectedThisSeason);
        player.Write(DataKeys.ConservationistTrashCollectedThisSeason, "0");
        if (trashCollectedThisSeason <= 0)
        {
            player.Write(DataKeys.ConservationistActiveTaxBonusPct, "0");
            return;
        }

        var taxBonusForNextSeason =
            // ReSharper disable once PossibleLossOfFraction
            Math.Min(
                trashCollectedThisSeason / ProfessionsModule.Config.TrashNeededPerTaxDeductionPct / 100f,
                ProfessionsModule.Config.ConservationistTaxBonusCeiling);
        player.Write(
            DataKeys.ConservationistActiveTaxBonusPct,
            taxBonusForNextSeason.ToString(CultureInfo.InvariantCulture));
        if (taxBonusForNextSeason <= 0 || TaxesModule.ShouldEnable)
        {
            return;
        }

        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
        player.mailForTomorrow.Add($"{Manifest.UniqueID}/ConservationistTaxNotice");
    }
}
