namespace DaLion.Ligo.Modules.Professions.Events.GameLoop;

#region using directives

using System.Globalization;
using System.Linq;
using DaLion.Ligo.Modules.Professions.Extensions;
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
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        if (!Context.IsMainPlayer)
        {
            return;
        }

        var conservationists = Game1.getAllFarmers().Where(f => f.HasProfession(Profession.Conservationist)).ToList();
        if (!ModEntry.Config.EnableTaxes)
        {
            foreach (var farmer in conservationists)
            {
                var taxBonus = farmer.Read<float>(DataFields.ConservationistActiveTaxBonusPct);
                if (taxBonus <= 0f)
                {
                    continue;
                }

                var amountSold = Game1.getFarm().getShippingBin(farmer).Sum(item =>
                    item is SObject obj ? obj.sellToStorePrice() * obj.Stack : item.salePrice() / 2);
                if (amountSold <= 0)
                {
                    continue;
                }

                farmer.Money += (int)(amountSold * taxBonus);
            }
        }

        if (Game1.dayOfMonth != 28)
        {
            return;
        }

        foreach (var farmer in conservationists)
        {
            var trashCollectedThisSeason =
                farmer.Read<uint>(DataFields.ConservationistTrashCollectedThisSeason);
            farmer.Write(DataFields.ConservationistTrashCollectedThisSeason, "0");
            if (trashCollectedThisSeason <= 0)
            {
                farmer.Write(DataFields.ConservationistActiveTaxBonusPct, "0");
                return;
            }

            var taxBonusForNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(
                    trashCollectedThisSeason / ModEntry.Config.Professions.TrashNeededPerTaxBonusPct / 100f,
                    ModEntry.Config.Professions.ConservationistTaxBonusCeiling);
            farmer.Write(
                DataFields.ConservationistActiveTaxBonusPct,
                taxBonusForNextSeason.ToString(CultureInfo.InvariantCulture));
            if (taxBonusForNextSeason <= 0 || ModEntry.Config.EnableTaxes)
            {
                continue;
            }

            ModEntry.ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
            farmer.mailForTomorrow.Add($"{ModEntry.Manifest.UniqueID}/ConservationistTaxNotice");
        }
    }
}
