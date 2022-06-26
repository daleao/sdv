namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using System;
using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common;
using Common.Data;
using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class RequestUpdateDataModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal RequestUpdateDataModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("RequestUpdateData")) return;

        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} tried to update the mod data.");
            return;
        }

        var split = e.Type.Split('/');
        var operation = split[1];
        var field = Enum.Parse<ModData>(split[2]);
        var value = e.ReadAs<string>();
        switch (operation)
        {
            case "Write":
                Log.D($"{who.Name} requested to Write {value} to {field}.");
                ModDataIO.WriteData(who, field.ToString(), value);
                break;

            case "Increment":
                Log.D($"{who.Name} requested to Increment {field} by {value}.");
                var parsedValue = e.ReadAs<int>();
                ModDataIO.IncrementData(who, field.ToString(), parsedValue);
                break;

            case "Append":
                Log.D($"{who.Name} requested to Append {value} to {field}.");
                ModDataIO.AppendData(who, field.ToString(), value);
                break;
        }
    }
}