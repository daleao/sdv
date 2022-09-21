namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using System.Linq;
using CommunityToolkit.Diagnostics;
using DaLion.Common;
using DaLion.Common.Attributes;
using DaLion.Common.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
[DebugOnly]
internal sealed class DebugModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="DebugModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager)
    {
        this.AlwaysEnabled = true;
    }

    /// <inheritdoc />
    public override bool Enable()
    {
        return false;
    }

    /// <inheritdoc />
    public override bool Disable()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("Debug"))
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
                        ModEntry.Broadcaster.Message(response, "Debug/Response", e.FromPlayerID);

                        break;
                }

                break;

            case "Response":
                Log.D($"Player {e.FromPlayerID} responded to {command} debug information.");
                if (ModEntry.Broadcaster.ResponseReceived is null)
                {
                    Log.E("But the response was null.");
                    return;
                }

                ModEntry.Broadcaster.ResponseReceived.TrySetResult(e.ReadAs<string>());

                break;
        }
    }
}
