using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace TheLion.Stardew.Professions.Framework.Events;

internal class SuperModeModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    public override void OnModMessageReceived(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID) return;

        var key = e.ReadAs<int>();
        if (!ModState.ActivePeerSuperModes.ContainsKey(key))
            ModState.ActivePeerSuperModes[key] = new();

        switch (e.Type)
        {
            case "SuperModeEnabled":
                ModEntry.Log($"Player {e.FromPlayerID} enabled Super Mode.", LogLevel.Trace);
                ModState.ActivePeerSuperModes[key].Add(e.FromPlayerID);
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

            case "SuperModeDisabled":
                ModEntry.Log($"Player {e.FromPlayerID}'s Super Mode has ended.", LogLevel.Trace);
                ModState.ActivePeerSuperModes[key].Remove(e.FromPlayerID);
                Game1.getFarmer(e.FromPlayerID).stopGlowing();
                break;
        }
    }
}