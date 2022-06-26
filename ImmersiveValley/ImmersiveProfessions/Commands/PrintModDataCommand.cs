namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Data;
using JetBrains.Annotations;
using StardewValley;
using System;
using static System.FormattableString;
using static System.String;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintModDataCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintModDataCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "data";

    /// <inheritdoc />
    public override string Documentation => "Print the current value of all mod data fields.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var message = $"Farmer {Game1.player.Name}'s mod data:";
        var value = ModDataIO.ReadData(Game1.player, ModData.EcologistItemsForaged.ToString());
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"{ModData.EcologistItemsForaged}: {value} ({ModEntry.Config.ForagesNeededForBestQuality - int.Parse(value)} needed for best quality)"
                       : $"Mod data does not contain an entry for {ModData.EcologistItemsForaged}.");

        value = ModDataIO.ReadData(Game1.player, ModData.GemologistMineralsCollected.ToString());
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"{ModData.GemologistMineralsCollected}: {value} ({ModEntry.Config.MineralsNeededForBestQuality - int.Parse(value)} needed for best quality)"
                       : $"Mod data does not contain an entry for {ModData.GemologistMineralsCollected}.");

        value = ModDataIO.ReadData(Game1.player, ModData.ProspectorHuntStreak.ToString());
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"{ModData.ProspectorHuntStreak}: {value} (affects treasure quality)"
                       : $"Mod data does not contain an entry for {ModData.ProspectorHuntStreak}.");

        value = ModDataIO.ReadData(Game1.player, ModData.ScavengerHuntStreak.ToString());
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? $"{ModData.ScavengerHuntStreak}: {value} (affects treasure quality)"
                       : $"Mod data does not contain an entry for {ModData.ScavengerHuntStreak}.");

        value = ModDataIO.ReadData(Game1.player, ModData.ConservationistTrashCollectedThisSeason.ToString());
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? CurrentCulture(
                           // ReSharper disable once PossibleLossOfFraction
                           $"{ModData.ConservationistTrashCollectedThisSeason}: {value} (expect a {Math.Min(int.Parse(value) / ModEntry.Config.TrashNeededPerTaxBonusPct / 100f, ModEntry.Config.ConservationistTaxBonusCeiling):p0} tax deduction next season)")
                       : $"Mod data does not contain an entry for {ModData.ConservationistTrashCollectedThisSeason}.");

        value = ModDataIO.ReadData(Game1.player, ModData.ConservationistActiveTaxBonusPct.ToString());
        message += "\n\t- " +
                   (!IsNullOrEmpty(value)
                       ? CurrentCulture($"{ModData.ConservationistActiveTaxBonusPct}: {float.Parse(value):p0}")
                       : $"Mod data does not contain an entry for {ModData.ConservationistActiveTaxBonusPct}.");

        Log.I(message);
    }
}