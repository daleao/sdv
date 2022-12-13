namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

[UsedImplicitly]
internal sealed class SetModDataCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SetModDataCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetModDataCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_data" };

    /// <inheritdoc />
    public override string Documentation => "Set a new value for the specified mod data field." + this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length == 0)
        {
            Log.W("You must specify a data field and value." + this.GetUsage());
            return;
        }

        var reset = args.Any(a => a is "clear" or "reset");
        if (reset)
        {
            SetEcologistItemsForaged(null);
            SetGemologistMineralsCollected(null);
            SetProspectorHuntStreak(null);
            SetScavengerHuntStreak(null);
            SetConservationistTrashCollectedThisSeason(null);
            Log.I("All data fields were reset.");
            return;
        }

        if (args.Length % 2 != 0)
        {
            Log.W("You must specify a data field and value." + this.GetUsage());
            return;
        }

        if (!int.TryParse(args[1], out var value) || value < 0)
        {
            Log.W("You must specify a positive integer value.");
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

    #region data setters

    private static void SetEcologistItemsForaged(int? value)
    {
        if (!Game1.player.HasProfession(Profession.Ecologist))
        {
            Log.W("You must have the Ecologist profession.");
            return;
        }

        Game1.player.Write(DataFields.EcologistItemsForaged, value?.ToString());
        if (value.HasValue)
        {
            Log.I($"Items foraged as Ecologist was set to {value}.");
        }
    }

    private static void SetGemologistMineralsCollected(int? value)
    {
        if (!Game1.player.HasProfession(Profession.Gemologist))
        {
            Log.W("You must have the Gemologist profession.");
            return;
        }

        Game1.player.Write(DataFields.GemologistMineralsCollected, value?.ToString());
        if (value.HasValue)
        {
            Log.I($"Minerals collected as Gemologist was set to {value}.");
        }
    }

    private static void SetProspectorHuntStreak(int? value)
    {
        if (!Game1.player.HasProfession(Profession.Prospector))
        {
            Log.W("You must have the Prospector profession.");
            return;
        }

        Game1.player.Write(DataFields.ProspectorHuntStreak, value?.ToString());
        if (value.HasValue)
        {
            Log.I($"Prospector Hunt was streak set to {value}.");
        }
    }

    private static void SetScavengerHuntStreak(int? value)
    {
        if (!Game1.player.HasProfession(Profession.Scavenger))
        {
            Log.W("You must have the Scavenger profession.");
            return;
        }

        Game1.player.Write(DataFields.ScavengerHuntStreak, value?.ToString());
        if (value.HasValue)
        {
            Log.I($"Scavenger Hunt streak was set to {value}.");
        }
    }

    private static void SetConservationistTrashCollectedThisSeason(int? value)
    {
        if (!Game1.player.HasProfession(Profession.Conservationist))
        {
            Log.W("You must have the Conservationist profession.");
            return;
        }

        Game1.player.Write(DataFields.ConservationistTrashCollectedThisSeason, value?.ToString());
        if (value.HasValue)
        {
            Log.I($"Conservationist trash collected in the current season was set to {value}.");
        }
    }

    #endregion data setters

    private static string GetAvailableFields()
    {
        var result = "\n\nAvailable data fields:";
        result += "\n\t- EcologistItemsForaged (shortcut 'forages')";
        result += "\n\t- GemologistMineralsCollected (shortcut 'minerals')";
        result += "\n\t- ProspectorHuntStreak (shortcut 'phunt')";
        result += "\n\t- ScavengerHuntStreak (shortcut 'shunt')";
        result += "\n\t- ConservationistTrashCollectedThisSeason (shortcut 'trash')";
        return result;
    }

    private string GetUsage()
    {
        var result = $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers.First()} <field> <value>";
        result += "\n\nParameters:";
        result += "\n\t<field>\t- the name of the field";
        result += "\\n\t<value>\t- the desired new value";
        result += "\n\nExamples:";
        result += $"\n\t{this.Handler.EntryCommand} {this.Triggers.First()} EcologistItemsForaged 100";
        result += $"\n\t{this.Handler.EntryCommand} {this.Triggers.First()} trash 500";
        result += "\n\nAvailable data fields:";
        result += "\n\t- EcologistItemsForaged (shortcut 'forages')";
        result += "\n\t- GemologistMineralsCollected (shortcut 'minerals')";
        result += "\n\t- ProspectorHuntStreak (shortcut 'phunt')";
        result += "\n\t- ScavengerHuntStreak (shortcut 'shunt')";
        result += "\n\t- ConservationistTrashCollectedThisSeason (shortcut 'trash')";
        result += GetAvailableFields();
        return result;
    }
}
