namespace DaLion.Stardew.Professions.Framework;

#region using directives

using Common;
using Common.Events;
using Events.GameLoop;
using Events.Input;
using Events.Multiplayer;
using Events.Player;
using Events.TreasureHunt;
using Events.Ultimate;
using Extensions;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using TreasureHunts;
using Ultimates;

#endregion using directives

/// <summary>Manages dynamic hooking and unhooking of profession events.</summary>
internal class ProfessionEventManager : EventManager
{
    /// <summary>Look-up of event types required by each profession.</summary>
    private readonly Dictionary<Profession, List<Type>> EventsByProfession = new()
    {
        { Profession.Brute, new() { typeof(BruteWarpedEvent) } },
        { Profession.Conservationist, new() { typeof(HostConservationismDayEndingEvent) } },
        { Profession.Desperado, new() { typeof(DesperadoUpdateTickedEvent) } },
        { Profession.Piper, new() { typeof(PiperWarpedEvent) } },
        { Profession.Prospector, new() { typeof(ProspectorHuntDayStartedEvent), typeof(ProspectorWarpedEvent), typeof(TrackerButtonsChangedEvent) } },
        { Profession.Scavenger, new() { typeof(ScavengerHuntDayStartedEvent), typeof(ScavengerWarpedEvent), typeof(TrackerButtonsChangedEvent) } },
        { Profession.Spelunker, new() { typeof(SpelunkerWarpedEvent) } }
    };

    /// <summary>Construct an instance.</summary>
    public ProfessionEventManager(IModEvents modEvents)
    : base(modEvents)
    {
        Log.D("[EventManager]: Hooking Profession mod events...");

        #region hookers

        foreach (var @event in ManagedEvents.OfType<UltimateActivatedEvent>())
            Ultimate.Activated += @event.OnActivated;

        foreach (var @event in ManagedEvents.OfType<UltimateChargeIncreasedEvent>())
            Ultimate.ChargeIncreased += @event.OnChargeIncreased;

        foreach (var @event in ManagedEvents.OfType<UltimateChargeInitiatedEvent>())
            Ultimate.ChargeInitiated += @event.OnChargeInitiated;

        foreach (var @event in ManagedEvents.OfType<UltimateDeactivatedEvent>())
            Ultimate.Deactivated += @event.OnDeactivated;

        foreach (var @event in ManagedEvents.OfType<UltimateEmptiedEvent>())
            Ultimate.Emptied += @event.OnEmptied;

        foreach (var @event in ManagedEvents.OfType<UltimateFullyChargedEvent>())
            Ultimate.FullyCharged += @event.OnFullyCharged;


        foreach (var @event in ManagedEvents.OfType<TreasureHuntEndedEvent>())
            TreasureHunt.Ended += @event.OnEnded;

        foreach (var @event in ManagedEvents.OfType<TreasureHuntStartedEvent>())
            TreasureHunt.Started += @event.OnStarted;

        Log.D("[EventManager]: Initialization of Profession Mod events completed.");

        #endregion hookers
    }

    /// <inheritdoc />
    internal override void HookForLocalPlayer()
    {
        Log.D($"[EventManager]: Hooking profession events for {Game1.player.Name}...");
        foreach (var pid in Game1.player.professions)
            try
            {
                if (Profession.TryFromValue(pid, out var profession))
                    HookForProfession(profession);
            }
            catch (IndexOutOfRangeException)
            {
                Log.D($"[EventManager]: Unexpected profession index {pid} will be ignored.");
            }

        if (Context.IsMultiplayer)
        {
            Log.D("[EventManager]: Hooking multiplayer events...");
            Hook<ToggledUltimateModMessageReceivedEvent>();
            if (Context.IsMainPlayer)
            {
                Hook<HostPeerConnectedEvent>();
                Hook<HostPeerDisconnectedEvent>();
            }
        }

        Log.D($"[EventManager]: Done hooking event for {Game1.player.Name}.");
    }

    /// <summary>Hook all events required by the specified profession.</summary>
    /// <param name="profession">A profession.</param>
    internal void HookForProfession(Profession profession)
    {
        if (profession == Profession.Conservationist && !Context.IsMainPlayer ||
            !EventsByProfession.TryGetValue(profession, out var events)) return;

        Log.D($"[EventManager]: Hooking events for {profession}...");
        Hook(events.ToArray());
    }

    /// <summary>Unhook all events related to the specified profession.</summary>
    /// <param name="profession">A profession.</param>
    internal void UnhookForProfession(Profession profession)
    {
        if (profession == Profession.Conservationist && Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist, out _)
            || !EventsByProfession.TryGetValue(profession, out var events)) return;

        if (profession == Profession.Spelunker) events.Add(typeof(SpelunkerUpdateTickedEvent));

        List<Type> except = new();
        if (profession == Profession.Prospector && Game1.player.HasProfession(Profession.Scavenger) ||
            profession == Profession.Scavenger && Game1.player.HasProfession(Profession.Prospector))
            except.Add(typeof(TrackerButtonsChangedEvent));

        Log.D($"[EventManager]: Unhooking {profession} events...");
        Unhook(events.Except(except).ToArray());
    }
}