namespace DaLion.Taxes.Commands;

#region using directives

using System.Text;
using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SetCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class SetCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["set"];

    /// <inheritdoc />
    public override string Documentation =>
        "Set the value of the specified data field. Note that these values are re-evaluated on a bi-monthly basis." +
        this.GetUsage();

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length != 2)
        {
            this.Handler.Log.W("You must specify a data key and value to be set.");
            return false;
        }

        if (!int.TryParse(args[1], out _))
        {
            this.Handler.Log.W($"'{args[1]}' is not a valid integer value.");
            return false;
        }

        var player = Game1.player;
        var farm = Game1.getFarm();
        switch (args[0].ToLower())
        {
            case "income":
                Data.Write(player, DataKeys.SeasonIncome, args[1]);
                this.Handler.Log.I($"{player.Name}'s season income has been set to {args[1]}.");
                break;
            case "expenses":
            case "deductibles":
                Data.Write(player, DataKeys.BusinessExpenses, args[1]);
                this.Handler.Log.I($"{player.Name}'s season business expenses has been set to {args[1]}.");
                break;
            case "debt":
            case "outstanding":
                Data.Write(player, DataKeys.DebtOutstanding, args[1]);
                this.Handler.Log.I($"{player.Name}'s debt has been set to {args[1]}.");
                break;
            case "agriculture":
                Data.Write(farm, DataKeys.AgricultureValue, args[1]);
                this.Handler.Log.I($"{player.farmName}'s agriculture valuation has been set to {args[1]}.");
                break;
            case "livestock":
                Data.Write(farm, DataKeys.LivestockValue, args[1]);
                this.Handler.Log.I($"{player.farmName}'s livestock valuation has been set to {args[1]}.");
                break;
            case "buildings":
                Data.Write(farm, DataKeys.BuildingValue, args[1]);
                this.Handler.Log.I($"{player.farmName}'s buildings' valuation has been set to {args[1]}.");
                break;
            case "usage":
                Data.Write(farm, DataKeys.UsedTiles, args[1]);
                this.Handler.Log.I($"{player.farmName}'s used tiles has been set to {args[1]}.");
                break;
            default:
                this.Handler.Log.I($"'{args[0]}' is not a valid data field.");
                break;
        }

        return true;
    }

    /// <inheritdoc />
    protected override string GetUsage()
    {
        var sb = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <field> <value>");
        sb.Append("\n\nParameters:");
        sb.Append("\n\t<field>\t- the name of the field");
        sb.Append("\n\t<value>\t- the desired new integer value");
        sb.Append("\n\nExamples:");
        sb.Append($"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} income 10000");
        sb.Append($"\n\t{this.Handler.EntryCommand} {this.Triggers[0]} debt 2500");
        sb.Append(this.GetAvailableKeys());
        return sb.ToString();
    }

    private string GetAvailableKeys()
    {
        var sb = new StringBuilder("\n\nAvailable data fields:");
        sb.Append("\n\t- <income>: the total income for the current season");
        sb.Append("\n\t- <expenses>: the total deductible business expenses ");
        sb.Append("\n\t- <debt> the total amount outstanding to be paid");
        sb.Append("\n\t- <agriculture>: the average agriculture value of the property so far the current year");
        sb.Append("\n\t- <livestock>: the average livestock value of the property so far the current year");
        sb.Append("\n\t- <usage>: the current amount of used tiles");
        return sb.ToString();
    }
}
