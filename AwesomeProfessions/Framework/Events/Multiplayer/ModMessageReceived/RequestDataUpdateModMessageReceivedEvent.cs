namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using System;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class RequestDataUpdateModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("RequestDataUpdate")) return;

        var split = e.Type.Split('/');
        var operation = split[1];
        var field = Enum.Parse<DataField>(split[2]);
        var value = e.ReadAs<string>();
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} tried to change the mod data.");
            return;
        }

        switch (operation)
        {
            case "Write":
                Log.D($"Player {e.FromPlayerID} requested to Write {value} to {field}.");
                ModData.Write(field, value, who);
                break;

            case "Increment":
                Log.D($"Player {e.FromPlayerID} requested to Increment {field} by {value}.");
                var parsedValue = e.ReadAs<int>();
                ModData.Increment(field, parsedValue, who);
                break;
        }
    }
}