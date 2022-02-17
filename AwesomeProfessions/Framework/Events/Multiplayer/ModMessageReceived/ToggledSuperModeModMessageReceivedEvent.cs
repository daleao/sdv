namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley;

using SuperMode;

#endregion using directives

internal class ToggledSuperModeModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("ToggledSuperMode")) return;

        var key = e.ReadAs<SuperModeIndex>();
        if (!ModEntry.State.Value.ActivePeerSuperModes.ContainsKey(key))
            ModEntry.State.Value.ActivePeerSuperModes[key] = new();

        switch (e.Type.Split('/')[1])
        {
            case "On":
                Log.D($"Player {e.FromPlayerID} has activated Super Mode.");
                ModEntry.State.Value.ActivePeerSuperModes[key].Add(e.FromPlayerID);
                var glowingColor = key switch
                {
                    SuperModeIndex.Brute => Color.OrangeRed,
                    SuperModeIndex.Poacher => Color.MediumPurple,
                    SuperModeIndex.Desperado => Color.DarkGoldenrod,
                    _ => Color.White
                };

                if (glowingColor != Color.White)
                    Game1.getFarmer(e.FromPlayerID).startGlowing(glowingColor, false, 0.05f);
                break;

            case "Off":
                Log.D($"Player {e.FromPlayerID}'s Super Mode has ended.");
                ModEntry.State.Value.ActivePeerSuperModes[key].Remove(e.FromPlayerID);
                Game1.getFarmer(e.FromPlayerID).stopGlowing();
                break;
        }
    }
}