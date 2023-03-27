namespace DaLion.Overhaul.Modules.Taxes.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class SendLetterCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SendLetterCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SendLetterCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "send", "letter" };

    /// <inheritdoc />
    public override string Documentation => "Trigger reception of the specified letter.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            Log.W("You must specify a letter to receive.");
            return;
        }

        if (args.Length > 1)
        {
            Log.W("Additional arguments will be ignored.");
        }

        switch (args[0].ToLowerInvariant())
        {
            case "frs.intro":
                PostalService.Send(Mail.FrsIntro);
                break;
            case "frs.notice":
                PostalService.Send(Mail.FrsNotice);
                break;
            case "frs.outstanding":
                PostalService.Send(Mail.FrsOutstanding);
                break;
            case "frs.deduction":
                PostalService.Send(Mail.FrsDeduction);
                break;
            case "lewis.notice":
                PostalService.Send(Mail.LewisNotice);
                break;
            case "lewis.outstanding":
                PostalService.Send(Mail.LewisOutstanding);
                break;
            default:
                Log.I($"'{args[0]}' is not a letter key.");
                break;
        }
    }
}
