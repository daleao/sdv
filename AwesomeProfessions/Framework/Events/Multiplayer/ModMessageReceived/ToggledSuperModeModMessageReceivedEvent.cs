namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using SuperMode;
using Extensions;

#endregion using directives

internal class ToggledSuperModeModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("ToggledSuperMode")) return;

        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} has toggled Super Mode.");
            return;
        }

        var state = e.ReadAs<string>();
        SuperModeIndex index;
        switch (state)
        {
            case "Active":
                Log.D($"{who.Name} has activated Super Mode.");
                index = who.ReadDataAs<SuperModeIndex>(DataField.SuperModeIndex);
                var glowingColor = index switch
                {
                    SuperModeIndex.Brute => Color.OrangeRed,
                    SuperModeIndex.Poacher => Color.MediumPurple,
                    SuperModeIndex.Desperado => Color.DarkGoldenrod,
                    _ => Color.White
                };

                if (glowingColor != Color.White)
                    who.startGlowing(glowingColor, false, 0.05f);

                if (Context.IsMainPlayer && index == SuperModeIndex.Poacher)
                    ModEntry.HostState.PoachersInAmbush.Add(e.FromPlayerID);

                break;

            case "Inactive":
                Log.D($"{who.Name}'s Super Mode has ended.");
                index = who.ReadDataAs<SuperModeIndex>(DataField.SuperModeIndex);
                who.stopGlowing();
                if (Context.IsMainPlayer && index == SuperModeIndex.Poacher)
                    ModEntry.HostState.PoachersInAmbush.Remove(e.FromPlayerID);

                break;
        }
    }
}