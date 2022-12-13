namespace DaLion.Overhaul.Modules.Core.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Events;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class PrintEnabledEventsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="PrintEnabledEventsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintEnabledEventsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "print_events", "events" };

    /// <inheritdoc />
    public override string Documentation => "Prints all currently subscribed mod events.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var message = "Enabled events:";
        var events = ModEntry.EventManager.Enabled.Cast<ManagedEvent>().ToList();
        events.Sort();
        message = events.Aggregate(
            message,
            (current, next) => current + "\n\t- " + next.GetType().Name);
        Log.I(message);
    }
}
