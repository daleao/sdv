namespace DaLion.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using System.Globalization;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.SMAPI;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ConservationismDayEndingEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ConservationismDayEndingEvent(EventManager? manager = null)
    : DayEndingEvent(manager ?? ProfessionsMod.Events)
{
    /// <inheritdoc />
    public override bool IsEnabled => Game1.dayOfMonth == 28 && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnDayEndingImpl(object? sender, DayEndingEventArgs e)
    {
        if (!Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist))
        {
            return;
        }

        var totalTrashCollectedThisSeason = 0;
        var totalTrashCollectedByOceanographer = 0;
        float taxBonusNextSeason;
        foreach (var player in Game1.getAllFarmers())
        {
            if (!player.HasProfession(Profession.Conservationist))
            {
                continue;
            }

            var trashCollectedByPlayerThisSeason =
                (int)Data.ReadAs<float>(player, DataKeys.ConservationistTrashCollectedThisSeason);
            Data.Write(player, DataKeys.ConservationistTrashCollectedThisSeason, "0");
            Data.Write(player, DataKeys.ConservationistTrashCollectedLastSeason, trashCollectedByPlayerThisSeason.ToString());
            totalTrashCollectedThisSeason += trashCollectedByPlayerThisSeason;
            if (player.HasProfession(Profession.Conservationist, true))
            {
                totalTrashCollectedByOceanographer += trashCollectedByPlayerThisSeason;
            }

            if (!player.useSeparateWallets)
            {
                continue;
            }

            if (totalTrashCollectedThisSeason == 0)
            {
                Data.Write(player, DataKeys.ActiveTaxDeduction, "0");
                continue;
            }

            taxBonusNextSeason =
                // ReSharper disable once PossibleLossOfFraction
                Math.Min(
                    totalTrashCollectedThisSeason / Config.ConservationistTrashNeededPerTaxDeduction / 100f,
                    Config.ConservationistTaxDeductionCeiling);
            Data.Write(
                player,
                DataKeys.ActiveTaxDeduction,
                taxBonusNextSeason.ToString(CultureInfo.InvariantCulture));
            if (taxBonusNextSeason <= 0f || !ModHelper.ModRegistry.IsLoaded("DaLion.Taxes"))
            {
                continue;
            }

            ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
            player.mailForTomorrow.Add($"{UniqueId}_ConservationistTaxNotice");
        }

        var host = Game1.player;
        var biodiversityFactor = Math.Min(totalTrashCollectedByOceanographer / 2000f, 1f);
        Data.Write(host, DataKeys.GlobalBiodiversityFactor, biodiversityFactor.ToString());

        if (host.useSeparateWallets)
        {
            return;
        }

        if (totalTrashCollectedThisSeason == 0)
        {
            foreach (var player in Game1.getAllFarmers())
            {
                Data.Write(player, DataKeys.ActiveTaxDeduction, "0");
            }

            return;
        }

        taxBonusNextSeason =
            // ReSharper disable once PossibleLossOfFraction
            Math.Min(
                totalTrashCollectedThisSeason / Config.ConservationistTrashNeededPerTaxDeduction / 100f,
                Config.ConservationistTaxDeductionCeiling);
        foreach (var player in Game1.getAllFarmers())
        {
            Data.Write(
                player,
                DataKeys.ActiveTaxDeduction,
                taxBonusNextSeason.ToString(CultureInfo.InvariantCulture));
        }

        if (taxBonusNextSeason <= 0f || !ModHelper.ModRegistry.IsLoaded("DaLion.Taxes"))
        {
            return;
        }

        ModHelper.GameContent.InvalidateCacheAndLocalized("Data/mail");
        host.mailForTomorrow.Add($"{UniqueId}_ConservationistTaxNotice");
    }
}
