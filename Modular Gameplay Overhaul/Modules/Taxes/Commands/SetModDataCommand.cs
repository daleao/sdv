namespace DaLion.Overhaul.Modules.Taxes.Commands;

#region using directives

using System.Linq;
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
    public override string Documentation => "Set the value of the specified mod data field.";

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
                    Game1.player.Write(DataKeys.SeasonIncome, args[0]);
                    Log.I($"{player.Name}'s season income has been set to {args[0]}.");
                    break;
                case "expenses":
                case "deductibles":
                    Game1.player.Write(DataKeys.BusinessExpenses, args[0]);
                    Log.I($"{player.Name}'s season business expenses has been set to {args[0]}.");
                    break;
                case "debt":
                    Game1.player.Write(DataKeys.DebtOutstanding, args[0]);
                    Log.I($"{player.Name}'s debt has been set to {args[0]}.");
                    break;
                case "agriculture":
                    Game1.getFarm().Write(DataKeys.AgricultureValue, args[0]);
                    Log.I($"{player.farmName}'s agriculture valuation has been set to {args[0]}.");
                    break;
                case "livestock":
                    Game1.getFarm().Write(DataKeys.LivestockValue, args[0]);
                    Log.I($"{player.farmName}'s livestock valuation has been set to {args[0]}.");
                    break;
                case "buildings":
                    Game1.getFarm().Write(DataKeys.BuildingValue, args[0]);
                    Log.I($"{player.farmName}'s buildings' valuation has been set to {args[0]}.");
                    break;
                case "usage":
                    Game1.getFarm().Write(DataKeys.BuildingValue, args[0]);
                    Log.I($"{player.farmName}'s buildings' valuation has been set to {args[0]}.");
                    break;
                default:
                    Log.I($"'{args[0]}' is not a valid data field.");
                    break;
            }

            args = args.Skip(2).ToArray();
        }
    }
}
