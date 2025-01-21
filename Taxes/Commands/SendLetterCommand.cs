namespace DaLion.Taxes.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SendLetterCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class SendLetterCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["send", "letter"];

    /// <inheritdoc />
    public override string Documentation => "Trigger reception of the specified letter.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            Log.W("You must specify a letter to receive.");
            return true;
        }

        switch (args[0].ToLower())
        {
            case "frs_intro":
                PostalService.Send(Mail.FrsIntro);
                break;
            case "frs_notice":
                PostalService.Send(Mail.FrsNotice);
                break;
            case "frs_outstanding":
                PostalService.Send(Mail.FrsOutstanding);
                break;
            case "frs_deduction":
                PostalService.Send(Mail.FrsDeduction);
                break;
            case "lewis_intro":
                PostalService.Send(Mail.LewisIntro);
                break;
            case "lewis_notice":
                PostalService.Send(Mail.LewisNotice);
                break;
            case "lewis_outstanding":
                PostalService.Send(Mail.LewisOutstanding);
                break;
            default:
                Log.I($"'{args[0]}' is not a letter key.");
                break;
        }

        return true;
    }
}
