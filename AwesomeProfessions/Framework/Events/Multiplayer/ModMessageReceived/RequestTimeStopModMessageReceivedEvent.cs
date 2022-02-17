namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

using GameLoop;

#endregion using directives

internal class RequestTimeStopModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("RequestTimeStop")) return;

        var split = e.Type.Split('/');
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} tried to stop time");
            return;
        }

        var operation = split[1];
        switch (operation)
        {
            case "Enable":
                Log.D($"Player {e.FromPlayerID} requested time stop.");
                EventManager.Enable(typeof(HostPrestigeTreasureHuntUpdateTickedEvent));
                break;

            case "Disable":
                Log.D($"Player {e.FromPlayerID} requested time stop.");
                EventManager.Disable(typeof(HostPrestigeTreasureHuntUpdateTickedEvent));
                break;
        }
    }
}