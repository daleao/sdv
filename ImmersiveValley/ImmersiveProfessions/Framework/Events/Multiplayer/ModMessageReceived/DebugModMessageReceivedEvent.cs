#if DEBUG
namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common;
using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class DebugModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal DebugModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("Debug")) return;

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
                    case "EventsHooked":
                        var response = Manager.Hooked.Aggregate("",
                            (current, next) => current + "\n\t- " + next.GetType().Name);
                        ModEntry.Broadcaster.Message(response, "Debug/Response",e.FromPlayerID);

                        break;
                }

                break;

            case "Response":
                Log.D($"Player {e.FromPlayerID} responded to {command} debug information.");
                ModEntry.Broadcaster.ResponseReceived.TrySetResult(e.ReadAs<string>());

                break;
        }
    }
}
#endif