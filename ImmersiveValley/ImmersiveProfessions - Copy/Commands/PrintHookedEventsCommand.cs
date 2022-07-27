namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using JetBrains.Annotations;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintHookedEventsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintHookedEventsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "print_events", "events" };

    /// <inheritdoc />
    public override string Documentation => "Print all currently subscribed mod events.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var message = "Hooked events:";
        message = ModEntry.EventManager.Hooked
            .Aggregate(message, (current, next) => current + "\n\t- " + next.GetType().Name);
        Log.I(message);
    }
}