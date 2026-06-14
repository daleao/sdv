namespace DaLion.Professions.Framework.Events.Multiplayer.ModMessageReceived;

#region using directives

using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="PiperMinionsModMessageReceivedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class PiperMinionsModMessageReceivedEvent(EventManager? manager = null)
    : ModMessageReceivedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != UniqueId)
        {
            return;
        }

        var split = e.Type.Split('_');
        if (split.Length != 2 || split[0] != "PiperMinions")
        {
            return;
        }

        var who = Game1.GetPlayer(e.FromPlayerID, onlyOnline: true);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} has toggled their LimitBreak ability.");
            return;
        }

        var action = split[1];
        switch (action)
        {
            case "spawn":
                var numberToSpawn = e.ReadAs<int>();
                who.SpawnMinions(numberToSpawn);
                break;

            case "dismiss":
                who.DismissMinions();
                break;
        }
    }
}
