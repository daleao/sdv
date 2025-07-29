namespace DaLion.Professions.Framework.Events.Multiplayer.ModMessageReceived;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.UpdateTicked;
using DaLion.Professions.Framework.VirtualProperties;
using DaLion.Shared.Events;
using Microsoft.Xna.Framework;
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

        var hunt = e.ReadAs<string>().Split('/');
        var theHuntIsOn = bool.Parse(hunt[0]);
        if (!theHuntIsOn)
        {
            who.Get_TreasureHunt().IsHuntingTreasure.Value = false;
            who.Get_TreasureHunt().LocationNameOrUniqueName = string.Empty;
            who.Get_TreasureHunt().TreasureTile = Vector2.Zero;
        }

        who.Get_TreasureHunt().IsHuntingTreasure.Value = true;
        who.Get_TreasureHunt().LocationNameOrUniqueName = hunt[1];
        who.Get_TreasureHunt().TreasureTile = new Vector2(float.Parse(hunt[2]), float.Parse(hunt[3]));

        var profession = e.Type.Split('/')[1] == "Prospector" ? Profession.Prospector : Profession.Scavenger;
        if (who.HasProfession(profession, true))
        {
            this.Manager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
        }
    }
}
