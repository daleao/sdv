namespace DaLion.Overhaul.Modules.Taxes.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

[UsedImplicitly]
internal sealed class SetIncomeCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SetIncomeCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetIncomeCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_income", "income" };

    /// <inheritdoc />
    public override string Documentation => "Set the player's current season income to the specified value.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length == 0 || (!int.TryParse(args[0], out _) &&
                                 string.Equals(args[0], "clear", StringComparison.InvariantCultureIgnoreCase)))
        {
            Log.W("You must specify an integer value.");
            return;
        }

        if (args.Length > 1)
        {
            Log.W("Additional arguments will be ignored.");
        }

        Game1.player.Write(
            DataFields.SeasonIncome,
            string.Equals(args[0], "clear", StringComparison.InvariantCultureIgnoreCase) ? string.Empty : args[0]);
        Log.I($"{Game1.player.Name}'s season income has been set to {args[0]}.");
    }
}
