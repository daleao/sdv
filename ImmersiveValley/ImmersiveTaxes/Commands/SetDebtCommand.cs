namespace DaLion.Stardew.Taxes.Commands;

#region using directives

using Common.Extensions.Stardew;
using Common;
using Common.Commands;
using System;

#endregion using directives

[UsedImplicitly]
internal sealed class SetDebtCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetDebtCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_debt", "debt" };

    /// <inheritdoc />
    public override string Documentation => "Set the player's current debt outstanding to the specified value.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length <= 0 || !int.TryParse(args[0], out _) && string.Equals(args[0], "clear", StringComparison.InvariantCultureIgnoreCase))
        {
            Log.W("You must specify an integer value.");
            return;
        }

        if (args.Length > 1) Log.W("Additional arguments will be ignored.");


        Game1.player.Write("DebtOutstanding",
            string.Equals(args[0], "clear", StringComparison.InvariantCultureIgnoreCase) ? string.Empty : args[0]);
        Log.I($"{Game1.player.Name}'s outstanding debt has been set to {args[0]}.");
    }
}