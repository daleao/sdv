namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.DayEnding;
using DaLion.Professions.Framework.Events.GameLoop.DayStarted;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Professions.Framework.Events.Multiplayer.PeerConnected;
using DaLion.Professions.Framework.Events.Player;
using DaLion.Professions.Framework.Events.World.ObjectListChanged;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions.Collections;
using StardewModdingAPI.Events;
using StardewValley;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="ProfessionSaveLoadedEvent"/> class.</summary>
/// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionSaveLoadedEvent(EventManager? manager = null)
    : SaveLoadedEvent(manager ?? ProfessionsMod.EventManager)
{
    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;
        this.Manager.Manage<ProfessionsChangedEvent>(player);

        // revalidate skills
        Skill.List.ForEach(s => s.Revalidate());

        // load limit break
        var limitId = Data.ReadAs(player, DataKeys.LimitBreakId, -1);
        if (limitId > 0)
        {
            var limit = LimitBreak.FromId(limitId);
            if (!player.professions.Contains(limitId))
            {
                Log.W(
                    $"{player.Name} has the Limit Break \"{limit.Name}\" but is missing the corresponding profession. The limit will be unbroken.");
                Data.Write(player, DataKeys.LimitBreakId, null);
            }
            else
            {
                State.LimitBreak = limit;
            }
        }

        // initialize treasure hunts
        if (player.HasProfession(Profession.Prospector))
        {
            State.ProspectorHunt = new ProspectorHunt();
        }

        if (player.HasProfession(Profession.Scavenger))
        {
            State.ScavengerHunt = new ScavengerHunt();
        }

        if (player.HasProfession(Profession.Aquarist))
        {
            ModHelper.GameContent.InvalidateCache("Data/Objects");
        }

        this.Manager.Enable<RevalidateBuildingsDayStartedEvent>();

        if (!Context.IsMainPlayer)
        {
            return;
        }

        // enable multiplayer events
        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Luremaster))
        {
            this.Manager.Enable(
                typeof(LuremasterDayStartedEvent),
                typeof(LuremasterTimeChangedEvent));
        }
        else if (Context.IsMultiplayer)
        {
            this.Manager.Enable<LuremasterPeerConnectedEvent>();
        }

        if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Piper, true))
        {
            this.Manager.Enable<ChromaBallObjectListChangedEvent>();
        }
    }
}
