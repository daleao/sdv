namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Text;
using DaLion.Overhaul.Modules.Professions.Extensions;
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
    public override void Callback(string trigger, string[] args)
    {
        var player = Game1.player;
        var message = new StringBuilder($"Farmer {player.Name}'s mod data:");
        var value = player.Read(DataKeys.EcologistItemsForaged);
        message.Append("\n\t- ").Append(
                   !IsNullOrEmpty(value)
                       ? $"Ecologist Items Foraged: {value}\n\t\tExpected quality: {(ObjectQuality)player.GetEcologistForageQuality()}" +
                         (int.Parse(value) < ProfessionsModule.Config.ForagesNeededForBestQuality
                             ? $" ({ProfessionsModule.Config.ForagesNeededForBestQuality - int.Parse(value)} needed for best quality)"
                             : Empty)
                       : "Mod data does not contain an entry for EcologistItemsForaged.");

        value = player.Read(DataKeys.GemologistMineralsCollected);
        message.Append("\n\t- ").Append(
                   !IsNullOrEmpty(value)
                       ? $"Gemologist Minerals Collected: {value}\n\t\tExpected quality: {(ObjectQuality)player.GetGemologistMineralQuality()}" +
                         (int.Parse(value) < ProfessionsModule.Config.MineralsNeededForBestQuality
                             ? $" ({ProfessionsModule.Config.MineralsNeededForBestQuality - int.Parse(value)} needed for best quality)"
                             : Empty)
                       : "Mod data does not contain an entry for GemologistMineralsCollected.");

        value = player.Read(DataKeys.ProspectorHuntStreak);
        message.Append("\n\t- ").Append(
                   !IsNullOrEmpty(value)
                       ? $"Prospector Hunt Streak: {value} (affects Prospector Hunt treasure quality)"
                       : "Mod data does not contain an entry for ProspectorHuntStreak.");

        value = player.Read(DataKeys.ScavengerHuntStreak);
        message.Append("\n\t- ").Append(
                   !IsNullOrEmpty(value)
                       ? $"Scavenger Hunt Streak: {value} (affects Scavenger Hunt treasure quality)"
                       : "Mod data does not contain an entry for ScavengerHuntStreak.");

        value = player.Read(DataKeys.ConservationistTrashCollectedThisSeason);
        message.Append("\n\t- ").Append(
                   !IsNullOrEmpty(value)
                       ? $"Conservationist Trash Collected ({SeasonExtensions.Current()}): {value}\n\t\tExpected tax deduction for {SeasonExtensions.Next()}: " +
                         // ReSharper disable once PossibleLossOfFraction
                         $"{Math.Min((int)float.Parse(value) / ProfessionsModule.Config.TrashNeededPerTaxDeduction / 100f, ProfessionsModule.Config.ConservationistTaxDeductionCeiling):0%}"
                       : "Mod data does not contain an entry for ConservationistTrashCollectedThisSeason.");

        value = player.Read(DataKeys.ConservationistActiveTaxDeduction);
        message.Append("\n\t- ").Append(
                   !IsNullOrEmpty(value)
                       ? CurrentCulture($"ConservationistActiveTaxDeduction: {float.Parse(value):0%}")
                       : "Mod data does not contain an entry for ConservationistActiveTaxDeduction.");

        Log.I(message.ToString());
    }
}
