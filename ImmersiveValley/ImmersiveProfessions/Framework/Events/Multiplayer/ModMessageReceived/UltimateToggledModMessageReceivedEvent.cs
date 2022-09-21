namespace DaLion.Stardew.Professions.Framework.Events.Multiplayer;

#region using directives

using DaLion.Common;
using DaLion.Common.Events;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Professions.Framework.Ultimates;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateToggledModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateToggledModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="ProfessionEventManager"/> instance that manages this event.</param>
    internal UltimateToggledModMessageReceivedEvent(ProfessionEventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer;

    /// <inheritdoc />
    public override bool Enable()
    {
        return false;
    }

    /// <inheritdoc />
    public override bool Disable()
    {
        return false;
    }

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != ModEntry.Manifest.UniqueID || !e.Type.StartsWith("ToggledUltimate"))
        {
            return;
        }

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
                var index = who.Read<int>("UltimateIndex");
                var glowingColor = Ultimate.FromValue(index).GlowColor;
                who.startGlowing(glowingColor, false, 0.05f);

                break;

            case "Inactive":
                Log.D($"{who.Name}'s Ultimate ability has ended.");
                who.stopGlowing();

                break;
        }
    }
}
