namespace DaLion.Overhaul.Modules.Taxes.Commands;

#region using directives

using System.Linq;
using System.Text;
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
    public override string[] Triggers { get; } = { "set" };

    /// <inheritdoc />
    public override string Documentation =>
        "Set the value of the specified mod data field. Note that these values are re-evaluated on a bi-monthly basis." +
        this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            Log.W("You must specify at least one data field to be set.");
            return;
        }

        if (args.Length % 2 != 0)
        {
            Log.W("You must specify an integer value for each data field to be set.");
            return;
        }

        for (var i = 1; i < args.Length; i += 2)
        {
            if (int.TryParse(args[i], out _))
            {
                continue;
            }

            Log.W($"'{args[1]}' is not an integer value.");
            return;
        }

        var player = Game1.player;
        while (args.Length > 0)
        {
            switch (args[0].ToLowerInvariant())
            {
                case "income":
                    Game1.player.Write(DataKeys.SeasonIncome, args[1]);
                    Log.I($"{player.Name}'s season income has been set to {args[1]}.");
                    break;
                case "expenses":
                case "deductibles":
                    Game1.player.Write(DataKeys.BusinessExpenses, args[1]);
                    Log.I($"{player.Name}'s season business expenses has been set to {args[1]}.");
                    break;
                case "debt":
                case "outstanding":
                    Game1.player.Write(DataKeys.DebtOutstanding, args[1]);
                    Log.I($"{player.Name}'s debt has been set to {args[1]}.");
                    break;
                case "agriculture":
                    Game1.getFarm().Write(DataKeys.AgricultureValue, args[1]);
                    Log.I($"{player.farmName}'s agriculture valuation has been set to {args[1]}.");
                    break;
                case "livestock":
                    Game1.getFarm().Write(DataKeys.LivestockValue, args[1]);
                    Log.I($"{player.farmName}'s livestock valuation has been set to {args[1]}.");
                    break;
                case "buildings":
                    Game1.getFarm().Write(DataKeys.BuildingValue, args[1]);
                    Log.I($"{player.farmName}'s buildings' valuation has been set to {args[1]}.");
                    break;
                case "usage":
                    Game1.getFarm().Write(DataKeys.UsedTiles, args[1]);
                    Log.I($"{player.farmName}'s used tiles has been set to {args[1]}.");
                    break;
                default:
                    Log.I($"'{args[0]}' is not a valid data field.");
                    break;
            }

            args = args.Skip(2).ToArray();
        }
    }

    private static string GetAvailableFields()
    {
        var result = new StringBuilder("\n\nAvailable data fields:");
        result.Append("\n\t- <income>: the total income for the current season");
        result.Append("\n\t- <expenses>: the total deductible business expenses ");
        result.Append("\n\t- <debt> the total amount outstanding to be paid");
        result.Append("\n\t- <agriculture>: the average agriculture value of the property so far the current year");
        result.Append("\n\t- <livestock>: the average livestock value of the property so far the current year");
        result.Append("\n\t- <usage>: the current amount of used tiles");
        return result.ToString();
    }

    private string GetUsage()
    {
        var result = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <field> <value>");
        result.Append("\n\nParameters:");
        result.Append("\n\t<field>\t- the name of the field");
        result.Append("\n\t<value>\t- the desired new integer value");
        result.Append("\n\nExamples:");
        result.Append($"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} income 10000");
        result.Append($"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} debt 2500");
        result.Append(GetAvailableFields());
        return result.ToString();
    }
}
