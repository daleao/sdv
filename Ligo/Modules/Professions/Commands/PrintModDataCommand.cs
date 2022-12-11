namespace DaLion.Ligo.Modules.Professions.Commands;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Enums;
using DaLion.Shared.Extensions.Stardew;
using static System.FormattableString;
using static System.String;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintModDataCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="PrintModDataCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintModDataCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "print_data", "data" };

    /// <inheritdoc />
    public override string Documentation => "Print the current value of all mod data fields.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var player = Game1.player;
        var message = $"Farmer {player.Name}'s mod data:";
        var value = player.Read(DataFields.EcologistItemsForaged);
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"Ecologist Items Foraged: {value}\t\tExpected quality: {(Quality)player.GetEcologistForageQuality()}" +
                         (int.Parse(value) < ProfessionsModule.Config.ForagesNeededForBestQuality
                             ? $"({ProfessionsModule.Config.ForagesNeededForBestQuality - int.Parse(value)} needed for best quality)"
                             : Empty)
                       : "Mod data does not contain an entry for EcologistItemsForaged.");

        value = player.Read(DataFields.GemologistMineralsCollected);
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"Gemologist Minerals Collected: {value}\n\t\tExpected quality: {(Quality)player.GetGemologistMineralQuality()}" +
                         (int.Parse(value) < ProfessionsModule.Config.MineralsNeededForBestQuality
                             ? $"({ProfessionsModule.Config.MineralsNeededForBestQuality - int.Parse(value)} needed for best quality)"
                             : Empty)
                       : "Mod data does not contain an entry for GemologistMineralsCollected.");

        value = player.Read(DataFields.ProspectorHuntStreak);
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"Prospector Hunt Streak: {value} (affects Prospector Hunt treasure quality)"
                       : "Mod data does not contain an entry for ProspectorHuntStreak.");

        value = player.Read(DataFields.ScavengerHuntStreak);
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"Scavenger Hunt Streak: {value} (affects Scavenger Hunt treasure quality)"
                       : "Mod data does not contain an entry for ScavengerHuntStreak.");

        value = player.Read(DataFields.ConservationistTrashCollectedThisSeason);
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"Conservationist Trash Collected ({SeasonExtensions.Current()}): {value}\n\t\tExpected tax deduction for {SeasonExtensions.Next()}: " +
                         // ReSharper disable once PossibleLossOfFraction
                         $"{Math.Min(int.Parse(value) / ProfessionsModule.Config.TrashNeededPerTaxBonusPct / 100f, ProfessionsModule.Config.ConservationistTaxBonusCeiling):0%}"
                       : "Mod data does not contain an entry for ConservationistTrashCollectedThisSeason.");

        value = player.Read(DataFields.ConservationistActiveTaxBonusPct);
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? CurrentCulture($"ConservationistActiveTaxBonusPct: {float.Parse(value):0%}")
                       : "Mod data does not contain an entry for ConservationistActiveTaxBonusPct.");

        Log.I(message);
    }
}
