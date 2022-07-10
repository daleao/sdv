namespace DaLion.Stardew.Taxes.Framework.Events;

#region using directives

using Common;
using Common.Data;
using Common.Events;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

[UsedImplicitly]
internal sealed class TaxModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal TaxModMessageReceivedEvent(EventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("UpdateData")) return;

        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} tried to update the mod data.");
            return;
        }

        var split = e.Type.Split('/');
        var operation = split[1];
        var field = split[2];
        var value = e.ReadAs<string>();
        switch (operation)
        {
            case "Write":
                Log.D($"{who.Name} requested to Write {value} to {field}.");
                ModDataIO.WriteTo(who, field, value);
                break;

            case "Increment":
                Log.D($"{who.Name} requested to Increment {field} by {value}.");
                var parsedValue = e.ReadAs<int>();
                ModDataIO.Increment(who, field, parsedValue);
                break;

            case "Append":
                Log.D($"{who.Name} requested to Append {value} to {field}.");
                ModDataIO.AppendTo(who, field, value);
                break;
        }
    }
}