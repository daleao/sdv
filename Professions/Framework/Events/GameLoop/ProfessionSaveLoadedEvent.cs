namespace DaLion.Professions.Framework.Events.GameLoop;

#region using directives

using DaLion.Professions.Framework.Events.GameLoop.DayStarted;
using DaLion.Professions.Framework.Events.GameLoop.OneSecondUpdateTicked;
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

        // load and validate ordered professions
        var storedProfessions = Data.Read(player, DataKeys.OrderedProfessions);
        if (string.IsNullOrEmpty(storedProfessions))
        {
            Data.Write(player, DataKeys.OrderedProfessions, string.Join(',', player.professions));
            State.OrderedProfessions = [.. player.professions];
        }
        else
        {
            var professionsList = storedProfessions.ParseList<int>();
            if (professionsList.Count != player.professions.Count || !professionsList.All(player.professions.Contains))
            {
                Log.W(
                    $"Player {player.Name}'s professions does not match the stored list of professions. The stored professions will be reset.");
                Data.Write(player, DataKeys.OrderedProfessions, string.Join(',', player.professions));
                State.OrderedProfessions = [.. player.professions];
            }
            else
            {
                State.OrderedProfessions = professionsList;
            }
        }

        // load limit break
        var limitId = Data.Read(player, DataKeys.LimitBreakId);
        if (!string.IsNullOrEmpty(limitId))
        {
            var limit = LimitBreak.FromName(limitId)!;
            if (!player.professions.Contains(limit.Id))
            {
                Log.W(
                    $"{player.Name} has broken the limits of {limit.Name} but is missing the corresponding profession. The limit will be unbroken.");
                Data.Write(player, DataKeys.LimitBreakId, null);
            }
            else
            {
                State.LimitBreak = limit;
            }
        }

        // load other data
        if (player.HasProfession(Profession.Ecologist, true))
        {
            State.PrestigedEcologistBuffsLookup = Data
                .Read(player, DataKeys.PrestigedEcologistBuffLookup)
                .ParseDictionary<string, int>();
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

        // enable events
        if (Context.IsMainPlayer)
        {
            if (Game1.game1.DoesAnyPlayerHaveProfession(Profession.Luremaster))
            {
                this.Manager.Enable(
                    typeof(LuremasterDayStartedEvent),
                    typeof(LuremasterOneSecondUpdateTickedEvent),
                    typeof(LuremasterTimeChangedEvent));
            }
            else if (Context.IsMultiplayer)
            {
                this.Manager.Enable<LuremasterPeerConnectedEvent>();
            }

            this.Manager.Enable<RevalidateBuildingsDayStartedEvent>();
        }
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
