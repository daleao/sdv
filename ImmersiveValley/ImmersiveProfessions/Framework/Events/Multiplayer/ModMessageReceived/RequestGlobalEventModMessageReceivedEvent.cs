namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common;
using Common.Events;
using GameLoop;

#endregion using directives

[UsedImplicitly]
internal sealed class RequestGlobalEventModMessageReceivedEvent : ModMessageReceivedEvent
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
            case "Conservationism":
                Log.D($"{who.Name} requested {which} event subscription.");
                ModEntry.EventManager.Hook<HostConservationismDayEndingEvent>();
                break;
            case "HuntIsOn":
                Log.D($"Prestiged treasure hunter {who.Name} is hunting for treasure.");
                ModEntry.EventManager.Hook<HostPrestigeTreasureHuntUpdateTickedEvent>();
                break;
            case "HuntIsOff":
                Log.D($"{who.Name}'s hunt has ended.");
                ModEntry.EventManager.Unhook<HostPrestigeTreasureHuntUpdateTickedEvent>();
                break;
        }
    }
}