namespace DaLion.Stardew.Professions.Framework.Events;

#region using directives

using Common;
using Common.Events;
using Display;
using GameLoop;
using Input;
using Player;
using Extensions;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using TreasureHunt;
using Ultimate;

#endregion using directives

/// <summary>Manages dynamic enabling and disabling of profession events.</summary>
internal class ProfessionEventManager : EventManager
{
    /// <summary>Look-up of event types required by each profession.</summary>
    private readonly Dictionary<Profession, List<Type>> _EventsByProfession = new()
    {
        { Profession.Brute, new() { typeof(BruteWarpedEvent) } },
        { Profession.Conservationist, new() { typeof(ConservationismDayEndingEvent) } },
        { Profession.Piper, new() { typeof(PiperWarpedEvent) } },
        { Profession.Prospector, new() { typeof(ProspectorHuntDayStartedEvent), typeof(ProspectorRenderedHudEvent), typeof(ProspectorWarpedEvent), typeof(TrackerButtonsChangedEvent) } },
        { Profession.Scavenger, new() { typeof(ScavengerHuntDayStartedEvent), typeof(ScavengerRenderedHudEvent), typeof(ScavengerWarpedEvent), typeof(TrackerButtonsChangedEvent) } },
        { Profession.Spelunker, new() { typeof(SpelunkerWarpedEvent) } }
    };

    /// <summary>Construct an instance.</summary>
    public ProfessionEventManager(IModEvents modEvents)
    : base(modEvents)
    {
        Log.D("[EventManager]: Hooking Profession mod events...");

        #region hookers

        foreach (var @event in ManagedEvents.OfType<UltimateActivatedEvent>())
            Ultimates.Ultimate.Activated += @event.OnActivated;

        foreach (var @event in ManagedEvents.OfType<UltimateChargeIncreasedEvent>())
            Ultimates.Ultimate.ChargeIncreased += @event.OnChargeIncreased;

        foreach (var @event in ManagedEvents.OfType<UltimateChargeInitiatedEvent>())
            Ultimates.Ultimate.ChargeInitiated += @event.OnChargeInitiated;

        foreach (var @event in ManagedEvents.OfType<UltimateDeactivatedEvent>())
            Ultimates.Ultimate.Deactivated += @event.OnDeactivated;

        foreach (var @event in ManagedEvents.OfType<UltimateEmptiedEvent>())
            Ultimates.Ultimate.Emptied += @event.OnEmptied;

        foreach (var @event in ManagedEvents.OfType<UltimateFullyChargedEvent>())
            Ultimates.Ultimate.FullyCharged += @event.OnFullyCharged;

        foreach (var @event in ManagedEvents.OfType<TreasureHuntEndedEvent>())
            TreasureHunts.TreasureHunt.Ended += @event.OnEnded;

        foreach (var @event in ManagedEvents.OfType<TreasureHuntStartedEvent>())
            TreasureHunts.TreasureHunt.Started += @event.OnStarted;

        Log.D("[EventManager]: Initialization of Profession Mod events completed.");

        #endregion hookers
    }

    /// <summary>Enable events for the local player's professions.</summary>
    internal void EnableForLocalPlayer()
    {
        Log.D($"[EventManager]: Enabling profession events for {Game1.player.Name}...");
        foreach (var pid in Game1.player.professions)
            try
            {
                if (Profession.TryFromValue(pid, out var profession))
                    EnableForProfession(profession);
            }
            catch (IndexOutOfRangeException)
            {
                Log.D($"[EventManager]: Unexpected profession index {pid} will be ignored.");
            }

        Log.D($"[EventManager]: Done enabling event for {Game1.player.Name}.");
    }

    /// <summary>Enable all events required by the specified profession.</summary>
    /// <param name="profession">A profession.</param>
    internal void EnableForProfession(Profession profession)
    {
        if (profession == Profession.Conservationist && !Context.IsMainPlayer ||
            !_EventsByProfession.TryGetValue(profession, out var events)) return;

        Log.D($"[EventManager]: Enabling events for {profession}...");
        Enable(events.ToArray());
    }

    /// <summary>Disable all events related to the specified profession.</summary>
    /// <param name="profession">A profession.</param>
    internal void DisableForProfession(Profession profession)
    {
        if (profession == Profession.Conservationist && Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist, out _)
            || !_EventsByProfession.TryGetValue(profession, out var events)) return;

        if (profession == Profession.Spelunker) events.Add(typeof(SpelunkerUpdateTickedEvent));

        List<Type> except = new();
        if (profession == Profession.Prospector && Game1.player.HasProfession(Profession.Scavenger) ||
            profession == Profession.Scavenger && Game1.player.HasProfession(Profession.Prospector))
            except.Add(typeof(TrackerButtonsChangedEvent));

        Log.D($"[EventManager]: Disabling {profession} events...");
        Disable(events.Except(except).ToArray());
    }
}