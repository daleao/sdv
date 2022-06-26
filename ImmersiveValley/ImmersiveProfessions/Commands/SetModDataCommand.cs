namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Data;
using Extensions;
using Framework;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class SetModDataCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetModDataCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "set_data";

    /// <inheritdoc />
    public override string Documentation => "Set a new value for the specified mod data field." + GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (!args.Any() || args.Length != 2)
        {
            Log.W("You must specify a data field and value." + GetUsage());
            return;
        }

        if (!int.TryParse(args[1], out var value) || value < 0)
        {
            Log.W("You must specify a positive integer value.");
            return;
        }

        if (!Context.IsWorldReady)
        {
            Log.W("You must load a save first.");
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "forage":
            case "itemsforaged":
            case "ecologist":
            case "ecologistitemsforaged":
                SetEcologistItemsForaged(value);
                break;

            case "minerals":
            case "mineralscollected":
            case "gemologist":
            case "gemologistmineralscollected":
                SetGemologistMineralsCollected(value);
                break;

            case "shunt":
            case "scavengerhunt":
            case "scavenger":
            case "scavengerhuntstreak":
                SetScavengerHuntStreak(value);
                break;

            case "phunt":
            case "prospectorhunt":
            case "prospector":
            case "prospectorhuntstreak":
                SetProspectorHuntStreak(value);
                break;

            case "trash":
            case "trashcollected":
            case "conservationist":
            case "conservationisttrashcollectedthisseason":
                SetConservationistTrashCollectedThisSeason(value);
                break;

            default:
                var message = $"'{args[0]}' is not a settable data field." + GetAvailableFields();
                Log.W(message);
                break;
        }
    }

    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Trigger} <field> <value>";
        result += "\n\nParameters:";
        result += "\n\t<field>\t- the name of the field";
        result += "\\n\t<value>\t- the desired new value";
        result += "\n\nExamples:";
        result += $"\n\t{Handler.EntryCommand} {Trigger} EcologistItemsForaged 100";
        result += $"\n\t{Handler.EntryCommand} {Trigger} trash 500";
        result += "\n\nAvailable data fields:";
        result += $"\n\t- {ModData.EcologistItemsForaged} (shortcut 'forages')";
        result += $"\n\t- {ModData.GemologistMineralsCollected} (shortcut 'minerals')";
        result += $"\n\t- {ModData.ProspectorHuntStreak} (shortcut 'phunt')";
        result += $"\n\t- {ModData.ScavengerHuntStreak} (shortcut 'shunt')";
        result += $"\n\t- {ModData.ConservationistTrashCollectedThisSeason} (shortcut 'trash')";
        result += GetAvailableFields();
        return result;
    }

    private static string GetAvailableFields()
    {
        var result = "\n\nAvailable data fields:";
        result += $"\n\t- {ModData.EcologistItemsForaged} (shortcut 'forages')";
        result += $"\n\t- {ModData.GemologistMineralsCollected} (shortcut 'minerals')";
        result += $"\n\t- {ModData.ProspectorHuntStreak} (shortcut 'phunt')";
        result += $"\n\t- {ModData.ScavengerHuntStreak} (shortcut 'shunt')";
        result += $"\n\t- {ModData.ConservationistTrashCollectedThisSeason} (shortcut 'trash')";
        return result;
    }

    #region data setters

    private static void SetEcologistItemsForaged(int value)
    {
        if (!Game1.player.HasProfession(Profession.Ecologist))
        {
            Log.W("You must have the Ecologist profession.");
            return;
        }

        ModDataIO.WriteData(Game1.player, ModData.EcologistItemsForaged.ToString(), value.ToString());
        Log.I($"Items foraged as Ecologist was set to {value}.");
    }

    private static void SetGemologistMineralsCollected(int value)
    {
        if (!Game1.player.HasProfession(Profession.Gemologist))
        {
            Log.W("You must have the Gemologist profession.");
            return;
        }

        ModDataIO.WriteData(Game1.player, ModData.GemologistMineralsCollected.ToString(), value.ToString());
        Log.I($"Minerals collected as Gemologist was set to {value}.");
    }

    private static void SetProspectorHuntStreak(int value)
    {
        if (!Game1.player.HasProfession(Profession.Prospector))
        {
            Log.W("You must have the Prospector profession.");
            return;
        }

        ModDataIO.WriteData(Game1.player, ModData.ProspectorHuntStreak.ToString(), value.ToString());
        Log.I($"Prospector Hunt was streak set to {value}.");
    }

    private static void SetScavengerHuntStreak(int value)
    {
        if (!Game1.player.HasProfession(Profession.Scavenger))
        {
            Log.W("You must have the Scavenger profession.");
            return;
        }

        ModDataIO.WriteData(Game1.player, ModData.ScavengerHuntStreak.ToString(), value.ToString());
        Log.I($"Scavenger Hunt streak was set to {value}.");
    }

    private static void SetConservationistTrashCollectedThisSeason(int value)
    {
        if (!Game1.player.HasProfession(Profession.Conservationist))
        {
            Log.W("You must have the Conservationist profession.");
            return;
        }

        ModDataIO.WriteData(Game1.player, ModData.ConservationistTrashCollectedThisSeason.ToString(), value.ToString());
        Log.I($"Conservationist trash collected in the current season was set to {value}.");
    }

    #endregion data setters
}