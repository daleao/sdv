namespace DaLion.Professions.Framework.Events.GameLoop;

using DaLion.Professions.Framework.Events.GameLoop.DayEnding;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.DayStarted;
using DaLion.Professions.Framework.Events.GameLoop.TimeChanged;
using DaLion.Professions.Framework.Events.Multiplayer.PeerConnected;
using DaLion.Professions.Framework.Limits;
using DaLion.Professions.Framework.TreasureHunts;
using DaLion.Shared.Events;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using StardewModdingAPI.Events;
using StardewValley;
using World.ObjectListChanged;

#endregion using directives

[UsedImplicitly]
[AlwaysEnabledEvent]
internal sealed class ProfessionSaveLoadedEvent : SaveLoadedEvent
{
    /// <summary>Initializes a new instance of the <see cref="ProfessionSaveLoadedEvent"/> class.</summary>
    /// <param name="manager">The <see cref="EventManager"/> instance that manages this event.</param>
    internal ProfessionSaveLoadedEvent(EventManager? manager = null)
        : base(manager ?? ProfessionsMod.EventManager)
    {
    }

    /// <inheritdoc />
    protected override void OnSaveLoadedImpl(object? sender, SaveLoadedEventArgs e)
    {
        var player = Game1.player;
        player.professions.OnValueAdded += this.OnProfessionAdded;
        player.professions.OnValueRemoved += this.OnProfessionRemoved;

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

        if (!Context.IsMainPlayer)
        {
            return;
        }

        // enable events
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

        this.Manager.Enable<RevalidateBuildingsDayEndingEvent>();
    }

    /// <summary>Invoked when a profession is added to the local player.</summary>
    /// <param name="added">The index of the added profession.</param>
    private void OnProfessionAdded(int added)
    {
        if (State.OrderedProfessions.AddOrReplace(added))
        {
            if (Profession.TryFromValue(added, out var profession))
            {
                profession.OnAdded(Game1.player);
            }
            else if (Profession.TryFromValue(added - 100, out profession))
            {
                profession.OnAdded(Game1.player, true);
            }
        }

        Data.Write(Game1.player, DataKeys.OrderedProfessions, string.Join(',', State.OrderedProfessions));
        if (added.IsIn(Profession.GetRange(true)))
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }

    /// <summary>Invoked when a profession is removed from the local player.</summary>
    /// <param name="removed">The index of the removed profession.</param>
    private void OnProfessionRemoved(int removed)
    {
        if (State.OrderedProfessions.Remove(removed))
        {
            if (Profession.TryFromValue(removed, out var profession))
            {
                profession.OnRemoved(Game1.player);
            }
            else if (Profession.TryFromValue(removed - 100, out profession))
            {
                profession.OnRemoved(Game1.player, true);
            }
        }

        Data.Write(Game1.player, DataKeys.OrderedProfessions, string.Join(',', State.OrderedProfessions));
        if (removed.IsIn(Profession.GetRange(true)))
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }
}
