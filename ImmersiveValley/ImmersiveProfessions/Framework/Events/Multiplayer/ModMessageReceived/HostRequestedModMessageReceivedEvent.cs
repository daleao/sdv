namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Common;
using Common.Events;
using GameLoop;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class HostRequestedModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal HostRequestedModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || e.Type != "RequestHost" || !Context.IsMainPlayer) return;

        var split = e.ReadAs<string>().Split('/');
        var request = split[0];
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Received {request} request from unknown player {e.FromPlayerID}.");
            return;
        }

        switch (request)
        {
            case "Conservationism":
                Log.D($"{who.Name} requested Conservationism event subscription.");
                Manager.Enable<ConservationismDayEndingEvent>();
                break;
            case "HuntIsOn":
                Log.D($"Prestiged treasure hunter {who.Name} is hunting for treasure.");
                Manager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
                break;
        }
    }
}