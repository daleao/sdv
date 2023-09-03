namespace DaLion.Overhaul.Modules.Professions.Events.Multiplayer.ModMessageReceived;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HostRequestedModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="HostRequestedModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal HostRequestedModMessageReceivedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != Manifest.UniqueID || e.Type != "RequestHost")
        {
            return;
        }

        var request = e.ReadAs<string>().SplitWithoutAllocation('/')[0].ToString();
        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"[PROFS]: Received {request} request from unknown player {e.FromPlayerID}.");
            return;
        }

        switch (request)
        {
            case "HuntIsOn":
                Log.D($"[PROFS]: {who.Name} is hunting for treasure. Time will be frozen for the duration.");
                this.Manager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
                break;
        }
    }
}
