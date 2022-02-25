namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

internal class RequestUpdateHostStateModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
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
            case "ToggledAggressiveTargeting":
                Log.D($"{who.Name} toggled aggressive piping mode.");
                ModEntry.HostState.AggressivePipers.Add(e.FromPlayerID);
                break;

            case "ToggledPassiveTargeting":
                Log.D($"{who.Name} toggled passive piping mode.");
                ModEntry.HostState.AggressivePipers.Remove(e.FromPlayerID);
                break;

            case "ActivatedAmbush":
                Log.D($"{who.Name} is mounting an ambush.");
                ModEntry.HostState.PoachersInAmbush.Add(e.FromPlayerID);
                break;

            case "DeactivatedAmbush":
                Log.D($"{who.Name}' ambush has ended.");
                ModEntry.HostState.AggressivePipers.Remove(e.FromPlayerID);
                break;
        }
    }
}