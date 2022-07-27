namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Attributes;
using Common.Commands;
using System.Linq;

#endregion using directives

[UsedImplicitly, DebugOnly]
internal sealed class PrintEnabledEventsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintEnabledEventsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "print_events", "events" };

    /// <inheritdoc />
    public override string Documentation => "Print all currently subscribed mod events.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var message = "Enabled events:";
        message = ModEntry.EventManager.Enabled
            .Aggregate(message, (current, next) => current + "\n\t- " + next.GetType().Name);
        Log.I(message);
    }
}