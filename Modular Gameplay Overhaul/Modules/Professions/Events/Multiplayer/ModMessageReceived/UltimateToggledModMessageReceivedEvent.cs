namespace DaLion.Overhaul.Modules.Professions.Events.Multiplayer;

#region using directives

using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Stardew;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class UltimateToggledModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="UltimateToggledModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal UltimateToggledModMessageReceivedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != Manifest.UniqueID || !e.Type.StartsWith("ToggledUltimate"))
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
                var index = who.Read<int>(DataFields.UltimateIndex);
                var ultimate = Ultimate.FromValue(index);
                Log.D($"[Ultimate]: {who.Name} activated {ultimate.Name}.");
                who.startGlowing(ultimate.GlowColor, false, 0.05f);

                break;

            case "Inactive":
                Log.D($"[Ultimate]: {who.Name}'s Ultimate has ended.");
                who.stopGlowing();

                break;
        }
    }
}
