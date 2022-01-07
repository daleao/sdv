using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events.Multiplayer.ModMessageReceived;

internal class RequestDataUpdateModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    public override void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("RequestDataUpdate")) return;

        var split = e.Type.Split('/');
        var operation = split[1];
        var field = split[2];
        var value = e.ReadAs<string>();
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            ModEntry.Log($"Unknown player {e.FromPlayerID} tried to change the mod data.", LogLevel.Warn);
            return;
        }

        switch (operation)
        {
            case "Write":
                ModEntry.Log($"Player {e.FromPlayerID} requested to Write {value} to {field}.", LogLevel.Trace);
                ModData.Write(field, value, who);
                break;

            case "Increment":
                ModEntry.Log($"Player {e.FromPlayerID} requested to Increment {field} by {value}.", LogLevel.Trace);
                var parsedValue = e.ReadAs<int>();
                ModData.Increment(field, parsedValue, who);
                break;
        }
    }
}