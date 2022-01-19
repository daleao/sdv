namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

using GameLoop;

#endregion using directives

internal class RequestGlobalEventEnableModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("RequestEventEnable")) return;

        var which = e.ReadAs<string>();
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} requested {which} event subscription.");
            return;
        }

        switch (which)
        {
            case "Conservationist":
                Log.D($"Player {e.FromPlayerID} requested {which} event subscription.");
                EventManager.Enable(typeof(GlobalConservationistDayEndingEvent));
                break;
        }
    }
}