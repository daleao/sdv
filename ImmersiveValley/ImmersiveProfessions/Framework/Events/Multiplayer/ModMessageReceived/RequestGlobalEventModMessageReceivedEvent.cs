namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

using Content;
using GameLoop;

#endregion using directives

internal class RequestGlobalEventModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("RequestEvent")) return;

        var which = e.ReadAs<string>();
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} requested {which} event subscription.");
            return;
        }

        switch (which)
        {
            case "Aquarist":
                Log.D($"{who.Name} requested {which} event subscription.");
                EventManager.Enable(typeof(HostFishPondDataRequestedEvent));
                break;
            case "Conservationism":
                Log.D($"{who.Name} requested {which} event subscription.");
                EventManager.Enable(typeof(HostConservationismDayEndingEvent));
                break;
            case "HuntIsOn":
                Log.D($"{who.Name} is hunting for treasure.");
                EventManager.Enable(typeof(HostPrestigeTreasureHuntUpdateTickedEvent));
                break;
            case "HuntIsOff":
                Log.D($"{who.Name}'s hunt has ended.");
                EventManager.Disable(typeof(HostPrestigeTreasureHuntUpdateTickedEvent));
                break;
        }
    }
}