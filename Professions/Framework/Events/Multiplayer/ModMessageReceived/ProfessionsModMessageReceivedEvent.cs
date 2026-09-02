namespace DaLion.Professions.Framework.Events.Multiplayer.ModMessageReceived;

using DaLion.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.DayStarted;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Professions.Framework.Events.World.ObjectListChanged;
using DaLion.Shared.Events;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionsModMessageReceivedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
internal sealed class ProfessionsModMessageReceivedEvent(EventManager? manager = null)
    : ModMessageReceivedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    public override bool IsEnabled => Context.IsMainPlayer && Context.IsMultiplayer;

    /// <inheritdoc />
    protected override void OnModMessageReceivedImpl(object? sender, ModMessageReceivedEventArgs e)
    {
        if (e.FromModID != UniqueId || e.Type is not ("PeerProfessionGained" or "PeerProfessionLost"))
        {
            return;
        }

        var which = e.ReadAs<string>();
        var who = Game1.GetPlayer(e.FromPlayerID, onlyOnline: true);
        if (who is null)
        {
            Log.W($"Unknown player {e.FromPlayerID} just gained the {which} profession?");
            return;
        }

        Log.I($"Congratulations to {who.Name} on gaining the {which} profession!");
        switch (which)
        {
            case "Aquarist":
            case "Breeder":
            case "Producer":
            case "Piper":
                this.Manager.Enable<RevalidateBuildingsDayStartedEvent>();
                if (which == "Piper")
                {
                    if (e.Type.EndsWith("Gained"))
                    {
                        this.Manager.Enable<ChromaBallObjectListChangedEvent>();
                    }
                    else if (e.Type.EndsWith("Lost") && !Game1.game1.DoesAnyPlayerHaveProfession(Profession.Piper))
                    {
                        this.Manager.Disable<ChromaBallObjectListChangedEvent>();
                    }
                }

                break;
            case "Rancher":
                if (e.Type.EndsWith("Gained"))
                {
                    this.Manager.Enable(
                        typeof(NutritionDayStartedEvent),
                        typeof(NutritionDayEndingEvent));
                }
                else if (e.Type.EndsWith("Lost") && !Game1.game1.DoesAnyPlayerHaveProfession(Profession.Rancher))
                {
                    this.Manager.Disable(
                        typeof(LuremasterDayStartedEvent),
                        typeof(LuremasterTimeChangedEvent));
                }

                break;
            case "Luremaster":
                if (e.Type.EndsWith("Gained"))
                {
                    this.Manager.Enable(
                        typeof(LuremasterDayStartedEvent),
                        typeof(LuremasterTimeChangedEvent));
                }
                else if (e.Type.EndsWith("Lost") && !Game1.game1.DoesAnyPlayerHaveProfession(Profession.Luremaster))
                {
                    this.Manager.Disable(
                        typeof(LuremasterDayStartedEvent),
                        typeof(LuremasterTimeChangedEvent));
                }

                break;
        }
    }
}
