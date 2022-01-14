namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Multiplayer = Utility.Multiplayer;

#endregion using directives

[UsedImplicitly]
internal class DebugModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("Debug")) return;

        var command = e.Type.Split('/')[1];
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            ModEntry.Log($"Unknown player {e.FromPlayerID} sent debug {command} message.", LogLevel.Warn);
            return;
        }

        switch (command)
        {
            case "Request":
                ModEntry.Log($"Player {e.FromPlayerID} requested debug information.", LogLevel.Debug);
                var what = e.ReadAs<string>();
                switch (what)
                {
                    case "EventsEnabled":
                        var response = ModEntry.EventManager.GetAllEnabled()
                            .Aggregate("", (current, next) => current + "\n\t- " + next.GetType().Name);
                        ModEntry.ModHelper.Multiplayer.SendMessage(response, "Debug/Response",
                            new[] {ModEntry.Manifest.UniqueID},
                            new[] {e.FromPlayerID});

                        break;
                }

                break;

            case "Response":
                ModEntry.Log($"Player {e.FromPlayerID} responded to {command} debug information.", LogLevel.Debug);
                Multiplayer.ResponseReceived.TrySetResult(e.ReadAs<string>());

                break;
        }
    }
}