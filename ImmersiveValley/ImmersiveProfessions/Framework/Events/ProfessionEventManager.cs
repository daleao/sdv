namespace DaLion.Stardew.Professions.Framework.Events;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using DaLion.Common;
using DaLion.Common.Events;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework.Events.Display;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using DaLion.Stardew.Professions.Framework.Events.Input;
using DaLion.Stardew.Professions.Framework.Events.Player;
using DaLion.Stardew.Professions.Framework.Events.TreasureHunt;
using DaLion.Stardew.Professions.Framework.Events.Ultimate;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>Manages dynamic enabling and disabling of profession events.</summary>
internal class ProfessionEventManager : EventManager
{
    /// <summary>Look-up of event types required by each profession.</summary>
    private readonly Dictionary<Profession, List<Type>> _eventsByProfession = new()
    {
        { Profession.Brute, new List<Type> { typeof(BruteWarpedEvent) } },
        { Profession.Conservationist, new List<Type> { typeof(ConservationismDayEndingEvent) } },
        { Profession.Piper, new List<Type> { typeof(PiperWarpedEvent) } },
        { Profession.Prospector, new List<Type> { typeof(ProspectorHuntDayStartedEvent), typeof(ProspectorRenderedHudEvent), typeof(ProspectorWarpedEvent), typeof(TrackerButtonsChangedEvent), } },
        { Profession.Rascal, new List<Type> { typeof(RascalButtonPressedEvent), typeof(RascalButtonReleasedEvent) } },
        { Profession.Scavenger, new List<Type> { typeof(ScavengerHuntDayStartedEvent), typeof(ScavengerRenderedHudEvent), typeof(ScavengerWarpedEvent), typeof(TrackerButtonsChangedEvent), } },
        { Profession.Spelunker, new List<Type> { typeof(SpelunkerWarpedEvent) } },
    };

    /// <summary>Initializes a new instance of the <see cref="ProfessionEventManager"/> class.</summary>
    /// <param name="modEvents">The <see cref="IModEvents"/> API for the current mod.</param>
    public ProfessionEventManager(IModEvents modEvents)
        : base(modEvents)
    {
        Log.D("[EventManager]: Hooking Profession mod events...");

        #region hookers

        foreach (var @event in this.ManagedEvents.OfType<UltimateActivatedEvent>())
        {
            Ultimates.Ultimate.Activated += @event.OnActivated;
        }

        foreach (var @event in this.ManagedEvents.OfType<UltimateChargeIncreasedEvent>())
        {
            Ultimates.Ultimate.ChargeIncreased += @event.OnChargeIncreased;
        }

        foreach (var @event in this.ManagedEvents.OfType<UltimateChargeInitiatedEvent>())
        {
            Ultimates.Ultimate.ChargeInitiated += @event.OnChargeInitiated;
        }

        foreach (var @event in this.ManagedEvents.OfType<UltimateDeactivatedEvent>())
        {
            Ultimates.Ultimate.Deactivated += @event.OnDeactivated;
        }

        foreach (var @event in this.ManagedEvents.OfType<UltimateEmptiedEvent>())
        {
            Ultimates.Ultimate.Emptied += @event.OnEmptied;
        }

        foreach (var @event in this.ManagedEvents.OfType<UltimateFullyChargedEvent>())
        {
            Ultimates.Ultimate.FullyCharged += @event.OnFullyCharged;
        }

        foreach (var @event in this.ManagedEvents.OfType<TreasureHuntEndedEvent>())
        {
            TreasureHunts.TreasureHunt.Ended += @event.OnEnded;
        }

        foreach (var @event in this.ManagedEvents.OfType<TreasureHuntStartedEvent>())
        {
            TreasureHunts.TreasureHunt.Started += @event.OnStarted;
        }

        Log.D("[EventManager]: Initialization of Profession Mod events completed.");

        #endregion hookers
    }

    /// <summary>Enables events for the local player's professions.</summary>
    internal void EnableForLocalPlayer()
    {
        Log.D($"[EventManager]: Enabling profession events for {Game1.player.Name}...");
        foreach (var pid in Game1.player.professions)
        {
            try
            {
                if (Profession.TryFromValue(pid, out var profession))
                {
                    this.EnableForProfession(profession);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Log.D($"[EventManager]: Unexpected profession index {pid} will be ignored.");
            }
        }

        Log.D($"[EventManager]: Done enabling event for {Game1.player.Name}.");
    }

    /// <summary>Enables all events required by the specified <paramref name="profession"/>.</summary>
    /// <param name="profession">A profession.</param>
    internal void EnableForProfession(Profession profession)
    {
        if ((profession == Profession.Conservationist && !Context.IsMainPlayer) ||
            !this._eventsByProfession.TryGetValue(profession, out var events))
        {
            return;
        }

        Log.D($"[EventManager]: Enabling events for {profession}...");
        this.Enable(events.ToArray());
    }

    /// <summary>Disables all events related to the specified <paramref name="profession"/>.</summary>
    /// <param name="profession">A profession.</param>
    internal void DisableForProfession(Profession profession)
    {
        if ((profession == Profession.Conservationist &&
             Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist, out _))
            || !this._eventsByProfession.TryGetValue(profession, out var events))
        {
            return;
        }

        if (profession == Profession.Spelunker)
        {
            events.Add(typeof(SpelunkerUpdateTickedEvent));
        }

        List<Type> except = new();
        if ((profession == Profession.Prospector && Game1.player.HasProfession(Profession.Scavenger)) ||
            (profession == Profession.Scavenger && Game1.player.HasProfession(Profession.Prospector)))
        {
            except.Add(typeof(TrackerButtonsChangedEvent));
        }

        Log.D($"[EventManager]: Disabling {profession} events...");
        this.Disable(events.Except(except).ToArray());
    }
}
