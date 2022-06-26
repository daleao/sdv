namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Common;
using Common.Events;
using GameLoop;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class RequestGlobalEventModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal RequestGlobalEventModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
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
                Manager.Hook<HostConservationismDayEndingEvent>();
                break;
            case "HuntIsOn":
                Log.D($"Prestiged treasure hunter {who.Name} is hunting for treasure.");
                Manager.Hook<HostPrestigeTreasureHuntUpdateTickedEvent>();
                break;
            case "HuntIsOff":
                Log.D($"{who.Name}'s hunt has ended.");
                Manager.Unhook<HostPrestigeTreasureHuntUpdateTickedEvent>();
                break;
        }
    }
}