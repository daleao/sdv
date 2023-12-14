namespace DaLion.Overhaul.Modules.Professions.Events.Multiplayer.ModMessageReceived;

#region using directives

using DaLion.Overhaul.Modules.Professions.Events.GameLoop.UpdateTicked;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

[UsedImplicitly]
internal sealed class HuntingForTreasureModMessageReceivedEvent : ModMessageReceivedEvent
{
    /// <summary>Initializes a new instance of the <see cref="HuntingForTreasureModMessageReceivedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal HuntingForTreasureModMessageReceivedEvent(EventManager manager)
        : base(manager)
    {
    }

    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMultiplayer && Context.IsMainPlayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != Manifest.UniqueID || !e.Type.StartsWith(OverhaulModule.Professions.Namespace))
        {
            return;
        }

        var split = e.Type.Split('/');
        if (split[1] != "HuntingForTreasure")
        {
            return;
        }

        var who = Game1.getFarmer(e.FromPlayerID);
        if (who is null)
        {
            Log.W($"[PRFS]: Unknown player {e.FromPlayerID} has started a Treasure Hunt.");
            return;
        }

        var huntingState = e.ReadAs<bool>();
        who.Get_IsHuntingTreasure().Value = huntingState;
        if (!huntingState)
        {
            return;
        }

        var profession = split[2] == "Prospector" ? VanillaProfession.Prospector : VanillaProfession.Scavenger;
        if (who.HasProfession(profession, true))
        {
            this.Manager.Enable<PrestigeTreasureHuntUpdateTickedEvent>();
        }
    }
}
