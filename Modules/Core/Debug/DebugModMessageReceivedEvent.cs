namespace DaLion.Overhaul.Modules.Core.Debug;

#region using directives

using System.Linq;
using DaLion.Shared.Attributes;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class DebugModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebugModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal DebugModMessageReceivedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => State.DebugMode;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != Manifest.UniqueID || !e.Type.StartsWith("Debug"))
        {
            return;
        }

        var command = e.Type.Split('/')[1];
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} sent debug {command} message.");
            return;
        }

        switch (command)
        {
            case "Request":
                Log.D($"Player {e.FromPlayerID} requested debug information.");
                var what = e.ReadAs<string>();
                switch (what)
                {
                    case "EventsEnabled":
                        var response = this.Manager.Enabled.Aggregate(
                            string.Empty,
                            (current, next) => current + "\n\t- " + next.GetType().Name);
                        Broadcaster.MessagePeer(response, "Debug/Response", e.FromPlayerID);

                        break;
                }

                break;

            case "Response":
                Log.D($"Player {e.FromPlayerID} responded to {command} debug information.");
                if (Broadcaster.ResponseReceived is null)
                {
                    Log.E("But the response was null.");
                    return;
                }

                Broadcaster.ResponseReceived.TrySetResult(e.ReadAs<string>());

                break;
        }
    }
}
