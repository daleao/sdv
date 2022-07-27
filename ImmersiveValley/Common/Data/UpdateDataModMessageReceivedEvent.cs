namespace DaLion.Common.ModData;

#region using directives

using Common;
using Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UpdateDataModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UpdateDataModMessageReceivedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModDataIO.ModID || e.Type != "UpdateData") return;

        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} tried to update the mod data.");
            return;
        }

        var split = e.ReadAs<string>().Split('/');
        var operation = split[0];
        var field = split[1];
        var value = split[2];
        switch (operation)
        {
            case "Write":
                Log.D($"{who.Name} requested to Write {value} to {field}.");
                ModDataIO.Write(who, field, value);
                break;

            case "Increment":
                Log.D($"{who.Name} requested to Increment {field} by {value}.");
                var parsedValue = e.ReadAs<int>();
                ModDataIO.Increment(who, field, parsedValue);
                break;

            case "Append":
                Log.D($"{who.Name} requested to Append {value} to {field}.");
                ModDataIO.Append(who, field, value);
                break;
        }
    }
}