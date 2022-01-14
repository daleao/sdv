namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

using Professions = Utility.Professions;

#endregion using directives

internal class ToggledSuperModeModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("ToggledSuperMode")) return;

        var key = e.ReadAs<int>();
        if (!ModEntry.State.Value.ActivePeerSuperModes.ContainsKey(key))
            ModEntry.State.Value.ActivePeerSuperModes[key] = new();

        switch (e.Type.Split('/')[1])
        {
            case "On":
                ModEntry.Log($"Player {e.FromPlayerID} has enabled Super Mode.", ModEntry.DefaultLogLevel);
                ModEntry.State.Value.ActivePeerSuperModes[key].Add(e.FromPlayerID);
                var glowingColor = Professions.NameOf(key) switch
                {
                    "Brute" => Color.OrangeRed,
                    "Poacher" => Color.MediumPurple,
                    "Desperado" => Color.DarkGoldenrod,
                    "Piper" => Color.LimeGreen,
                    _ => Color.White
                };
                Game1.getFarmer(e.FromPlayerID).startGlowing(glowingColor, false, 0.05f);
                break;

            case "Off":
                ModEntry.Log($"Player {e.FromPlayerID}'s Super Mode has ended.", ModEntry.DefaultLogLevel);
                ModEntry.State.Value.ActivePeerSuperModes[key].Remove(e.FromPlayerID);
                Game1.getFarmer(e.FromPlayerID).stopGlowing();
                break;
        }
    }
}