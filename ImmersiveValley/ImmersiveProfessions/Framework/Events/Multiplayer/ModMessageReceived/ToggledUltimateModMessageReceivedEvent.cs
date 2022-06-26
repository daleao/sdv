namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Common;
using Common.Data;
using Common.Events;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class ToggledUltimateModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal ToggledUltimateModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("ToggledUltimate")) return;

        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} has toggled their Ultimate.");
            return;
        }

        var state = e.ReadAs<string>();
        UltimateIndex index;
        switch (state)
        {
            case "Active":
                Log.D($"{who.Name} activated their Ultimate.");
                index = ModDataIO.ReadDataAs<UltimateIndex>(who, ModData.UltimateIndex.ToString());
                var glowingColor = index switch
                {
                    UltimateIndex.BruteFrenzy => Color.OrangeRed,
                    UltimateIndex.PoacherAmbush => Color.MediumPurple,
                    UltimateIndex.DesperadoBlossom => Color.DarkGoldenrod,
                    _ => Color.White
                };

                if (glowingColor != Color.White)
                    who.startGlowing(glowingColor, false, 0.05f);

                if (Context.IsMainPlayer && index == UltimateIndex.PoacherAmbush)
                    ModEntry.HostState.PoachersInAmbush.Add(e.FromPlayerID);

                break;

            case "Inactive":
                Log.D($"{who.Name}'s Ultimate has ended.");
                index = ModDataIO.ReadDataAs<UltimateIndex>(who, ModData.UltimateIndex.ToString());
                who.stopGlowing();
                if (Context.IsMainPlayer && index == UltimateIndex.PoacherAmbush)
                    ModEntry.HostState.PoachersInAmbush.Remove(e.FromPlayerID);

                break;
        }
    }
}