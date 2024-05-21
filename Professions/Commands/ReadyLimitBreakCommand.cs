namespace DaLion.Professions.Commands;

#region using directives

using DaLion.Professions.Framework.Limits;
using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ReadyLimitBreakCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class ReadyLimitBreakCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["rdy"];

    /// <inheritdoc />
    public override string Documentation =>
        "Max-out the player's Limit Gauge, or set it to the specified percentage.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        var limit = State.LimitBreak;
        if (limit is null)
        {
            this.Handler.Log.W("You don't have a Limit Break.");
            return true;
        }

        switch (args.Length)
        {
            case <= 0:
                limit.ChargeValue = LimitBreak.MaxCharge;
                return true;

            case > 1:
                this.Handler.Log.W("Additional arguments will be ignored.");
                break;
        }

        if (!int.TryParse(args[0], out var value) || value is < 0 or > 100)
        {
            this.Handler.Log.W($"{value} should be a number between 0 and 100.");
            return true;
        }

        // ReSharper disable once PossibleLossOfFraction
        limit.ChargeValue = value / 100d * LimitBreak.MaxCharge;
        return true;
    }
}
