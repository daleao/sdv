namespace DaLion.Common.Commands;

#region using directives

using System.Linq;
using DaLion.Common.Attributes;
using DaLion.Common.Events;

#endregion using directives

[UsedImplicitly]
[DebugOnly]
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
    public override string Documentation => "Print all currently subscribed mod events.";

    internal static EventManager? Manager { get; set; }

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (Manager is null)
        {
            return;
        }

        var message = "Enabled events:";
        message = Manager.Enabled
            .Aggregate(message, (current, next) => current + "\n\t- " + next.GetType().Name);
        Log.I(message);
    }
}
