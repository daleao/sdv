namespace DaLion.Shared.Events;

#region using directives

using System.Text;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ListEventsCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
/// <param name="eventManager">The <see cref="EventManager"/> instance.</param>
[ImplicitIgnore]
[Debug]
internal sealed class ListEventsCommand(CommandHandler handler, EventManager eventManager)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["events"];

    /// <inheritdoc />
    public override string Documentation => "Lists currently enabled events.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        var output = new StringBuilder();

        if (args.Length > 0 && args[0] == "managed")
        {
            output.AppendLine("Currently managed events:");
            foreach (var @event in eventManager.Managed)
            {
                output.AppendLine($"\t- {@event.GetType().Name}");
            }

            Log.I(output.ToString());
            return true;
        }

        if (args.Length == 0 || args[0] == "enabled")
        {
            output.AppendLine("Currently enabled events:");
            foreach (var @event in eventManager.Enabled)
            {
                output.AppendLine($"\t- {@event.GetType().Name}");
            }

            Log.I(output.ToString());
            return true;
        }

        Log.W($"Invalid argument {args[0]}.");
        return false;
    }
}
