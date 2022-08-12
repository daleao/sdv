namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using Common;
using Common.Events;
using Common.Extensions.Stardew;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using Ultimates;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateToggledModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer;

    /// <summary>Construct an instance.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateToggledModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager) { }

    /// <inheritdoc />
    public override bool Enable() => false;

    /// <inheritdoc />
    public override bool Disable() => false;

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

        var newState = e.ReadAs<string>();
        switch (newState)
        {
            case "Active":
                Log.D($"{who.Name} activated their Ultimate ability.");
                var index = who.Read<UltimateIndex>("UltimateIndex");
                var glowingColor = index switch
                {
                    UltimateIndex.BruteFrenzy => Color.OrangeRed,
                    UltimateIndex.PoacherAmbush => Color.MediumPurple,
                    UltimateIndex.DesperadoBlossom => Color.DarkGoldenrod,
                    _ => Color.White
                };

                if (glowingColor != Color.White)
                    who.startGlowing(glowingColor, false, 0.05f);

                break;

            case "Inactive":
                Log.D($"{who.Name}'s Ultimate ability has ended.");
                who.stopGlowing();

                break;
        }
    }
}