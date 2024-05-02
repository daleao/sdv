namespace DaLion.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using System.Globalization;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class ConservationismDayEndingEvent : DayEndingEvent
{
    /// <summary>Initializes a new instance of the <see cref="ConservationismDayEndingEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ConservationismDayEndingEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Game1.dayOfMonth == 28 && Game1.player.HasProfession(Profession.Conservationist);

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        var player = Game1.player;
        var trashCollectedThisSeason =
            (int)Data.ReadAs<float>(player, DataKeys.ConservationistTrashCollectedThisSeason);
        Data.Write(player, DataKeys.ConservationistTrashCollectedThisSeason, "0");
        if (trashCollectedThisSeason <= 0f)
        {
            Data.Write(player, DataKeys.ConservationistActiveTaxDeduction, "0");
            return;
        }

        var taxBonusForNextSeason =
            // ReSharper disable once PossibleLossOfFraction
            Math.Min(
                trashCollectedThisSeason / Config.ConservationistTrashNeededPerTaxDeduction / 100f,
                Config.ConservationistTaxDeductionCeiling);
        Data.Write(
            player,
            DataKeys.ConservationistActiveTaxDeduction,
            taxBonusForNextSeason.ToString(CultureInfo.InvariantCulture));
        if (taxBonusForNextSeason <= 0f || !ModHelper.ModRegistry.IsLoaded("DaLion.Taxes"))
        {
            return;
        }

        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
        player.mailForTomorrow.Add($"{UniqueId}/ConservationistTaxNotice");
    }
}
