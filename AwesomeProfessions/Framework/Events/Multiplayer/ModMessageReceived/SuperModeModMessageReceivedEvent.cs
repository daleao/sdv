using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events.Multiplayer.ModMessageReceived;

internal class SuperModeModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    public override void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("SuperMode")) return;

        var key = e.ReadAs<int>();
        if (!ModEntry.State.Value.ActivePeerSuperModes.ContainsKey(key))
            ModEntry.State.Value.ActivePeerSuperModes[key] = new();

        switch (e.Type)
        {
            case "SuperMode/Enabled":
                ModEntry.Log($"Player {e.FromPlayerID} has enabled Super Mode.", LogLevel.Trace);
                ModEntry.State.Value.ActivePeerSuperModes[key].Add(e.FromPlayerID);
                var glowingColor = Utility.Professions.NameOf(key) switch
                {
                    "Brute" => Color.OrangeRed,
                    "Poacher" => Color.MediumPurple,
                    "Desperado" => Color.DarkGoldenrod,
                    "Piper" => Color.LimeGreen,
                    _ => Color.White
                };
                Game1.getFarmer(e.FromPlayerID).startGlowing(glowingColor, false, 0.05f);
                break;

            case "SuperMode/Disabled":
                ModEntry.Log($"Player {e.FromPlayerID}'s Super Mode has ended.", LogLevel.Trace);
                ModEntry.State.Value.ActivePeerSuperModes[key].Remove(e.FromPlayerID);
                Game1.getFarmer(e.FromPlayerID).stopGlowing();
                break;
        }
    }
}