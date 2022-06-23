namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System.Linq;

using Common;
using Common.Commands;

#endregion using directives

internal class PrintHookedEventsCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "events";

    /// <inheritdoc />
    public string Documentation => "Print all currently subscribed mod events.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        var message = "Hooked events:";
        message = ModEntry.EventManager.Hooked
            .Aggregate(message, (current, next) => current + "\n\t- " + next.GetType().Name);
        Log.I(message);
    }
}