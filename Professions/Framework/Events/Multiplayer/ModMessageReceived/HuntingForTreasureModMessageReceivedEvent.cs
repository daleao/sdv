namespace DaLion.Professions.Framework.Events.Multiplayer.ModMessageReceived;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="HuntingForTreasureModMessageReceivedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class HuntingForTreasureModMessageReceivedEvent(EventManager? manager = null)
    : ModMessageReceivedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != UniqueId || !e.Type.StartsWith("HuntingForTreasure"))
        {
            return;
        }

        var who = Game1.GetPlayer(e.FromPlayerID, onlyOnline: true);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} has started a Treasure Hunt.");
            return;
        }

        var isHunting = e.ReadAs<bool>();
        who.Get_IsHuntingTreasure().Value = isHunting;
        if (!isHunting)
        {
            return;
        }

        var profession = e.Type.Split('/')[1] == "Prospector" ? Profession.Prospector : Profession.Scavenger;
        if (who.HasProfession(profession, true))
        {
            this.Manager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
        }
    }
}
