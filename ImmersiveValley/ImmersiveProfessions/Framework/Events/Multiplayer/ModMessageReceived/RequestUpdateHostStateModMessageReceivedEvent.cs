namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using JetBrains.Annotations;
using StardewModdingAPI.Events;
using StardewValley;

using Common;
using Common.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class RequestUpdateHostStateModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal RequestUpdateHostStateModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("RequestUpdateHostState")) return;

        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} tried to update the host state.");
            return;
        }

        var operation = e.ReadAs<string>();
        switch (operation)
        {
            case "ActivatedAmbush":
                Log.D($"{who.Name} is mounting an ambush.");
                ModEntry.HostState.PoachersInAmbush.Add(e.FromPlayerID);
                break;

            case "DeactivatedAmbush":
                Log.D($"{who.Name}' ambush has ended.");
                ModEntry.HostState.PoachersInAmbush.Remove(e.FromPlayerID);
                break;
        }
    }
}