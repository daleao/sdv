using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Common.Harmony;
using TheLion.Stardew.Professions.Framework.Events;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework;

/// <summary>Manages dynamic subscribing and unsubscribing of events for modded professions.</summary>
internal class EventSubscriber : IEnumerable<IEvent>
{
    private static readonly Dictionary<string, List<IEvent>> EventsByProfession = new()
    {
        {"Conservationist", new() {new ConservationistDayEndingEvent()}},
        {"Poacher", new() {new PoacherWarpedEvent()}},
        {"Piper", new() {new PiperWarpedEvent()}},
        {"Prospector", new() {new ProspectorHuntDayStartedEvent(), new ProspectorWarpedEvent(), new TrackerButtonsChangedEvent()}},
        {"Scavenger", new() {new ScavengerHuntDayStartedEvent(), new ScavengerWarpedEvent(), new TrackerButtonsChangedEvent()}},
        {"Spelunker", new() {new SpelunkerWarpedEvent()}}
    };

    private readonly List<IEvent> _subscribed = new();

    /// <summary>Construct an instance.</summary>
    internal EventSubscriber()
    {
        // hook static events
        SubscribeEventsStartingWith("Static");

        // hook debug events
        if (ModEntry.Config.EnableUIDebug) SubscribeEventsStartingWith("Debug");
    }

    public IEnumerator<IEvent> GetEnumerator()
    {
        return _subscribed.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _subscribed.GetEnumerator();
    }

    /// <summary>Subscribe new events to the event listener.</summary>
    /// <param name="events">Events to be subscribed.</param>
    internal void Subscribe(params IEvent[] events)
    {
        foreach (var e in events)
            if (_subscribed.ContainsType(e.GetType()))
            {
                ModEntry.Log($"[EventSubscriber]: Farmer already subscribed to {e.GetType().Name}.",
                    LogLevel.Trace);
            }
            else
            {
                e.Hook();
                _subscribed.Add(e);
                ModEntry.Log($"[EventSubscriber]: Subscribed to {e.GetType().Name}.", LogLevel.Trace);
            }
    }

    /// <summary>Unsubscribe events from the event listener.</summary>
    /// <param name="eventTypes">The event types to be unsubscribed.</param>
    internal void Unsubscribe(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
            if (_subscribed.RemoveType(type, out var removed))
            {
                removed.Unhook();
                ModEntry.Log($"[EventSubscriber]: Unsubscribed from {type.Name}.", LogLevel.Trace);
            }
            else
            {
                ModEntry.Log($"[EventSubscriber]: Farmer not subscribed to {type.Name}.", LogLevel.Trace);
            }
    }

    /// <summary>
    ///     Search the assembly for event types whose names start with a given prefix, and subscribe to an instance of
    ///     those events.
    /// </summary>
    /// <paramref name="prefix">One of <c>Static</c> or <c>Debug</c>.</paramref>
    internal void SubscribeEventsStartingWith(string prefix, params Type[] except)
    {
        ModEntry.Log($"[EventSubscriber]: Subscribing {prefix} events using reflection...", LogLevel.Trace);
        var eventsToSubscribe = AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(IEvent)))
            .Where(t => t.IsAssignableTo(typeof(IEvent)) && !t.IsAbstract &&
                        t.Name.StartsWith(prefix))
            .Except(except)
            .Select(t => (IEvent) t.Constructor().Invoke(Array.Empty<object>()))
            .ToArray();
        Subscribe(eventsToSubscribe);
    }

    /// <summary>Subscribe the event listener to events required for prestige functionality.</summary>
    internal void UnsubscribeEventsStartingWith(string prefix, params Type[] except)
    {
        ModEntry.Log($"[EventSubscriber]: Unsubscribing {prefix} events...", LogLevel.Trace);
        var eventsToRemove = _subscribed
            .Select(e => e.GetType())
            .Where(t => t.Name.StartsWith(prefix))
            .Except(except)
            .ToArray();
        Unsubscribe(eventsToRemove);
    }

    /// <summary>Subscribe the event listener to all events required by the local player's current professions.</summary>
    internal void SubscribeEventsForLocalPlayer()
    {
        ModEntry.Log($"[EventSubscriber]: Subscribing dynamic events for farmer {Game1.player.Name}...",
            LogLevel.Trace);
        foreach (var professionIndex in Game1.player.professions)
            try
            {
                SubscribeEventsForProfession(Utility.Professions.NameOf(professionIndex));
            }
            catch (IndexOutOfRangeException)
            {
                ModEntry.Log($"[EventSubscriber]: Unexpected profession index {professionIndex} will be ignored.",
                    LogLevel.Trace);
            }

        ModEntry.Log("[EventSubscriber]: Done subscribing profession events.", LogLevel.Trace);
        Subscribe(new SuperModeIndexChangedEvent());
        ModEntry.Log("[EventSubscriber]: Done subscribing player events.", LogLevel.Trace);
    }

    /// <summary>Unsubscribe the event listener from all non-static events.</summary>
    internal void UnsubscribeLocalPlayerEvents()
    {
        ModEntry.Log("[EventSubscriber]: Unsubscribing player dynamic events...", LogLevel.Trace);
        var eventsToRemove = _subscribed
            .Where(e => !e.GetType().Name.SplitCamelCase().First().IsAnyOf("Static", "Debug"))
            .Select(subscribed => subscribed.GetType())
            .AddItem(typeof(SuperModeIndexChangedEvent))
            .ToArray();
        Unsubscribe(eventsToRemove);
        ModEntry.Log("[EventSubscriber]: Done unsubscribing player events.", LogLevel.Trace);
    }

    /// <summary>Subscribe the event listener to all events required by a specific profession.</summary>
    /// <param name="whichProfession">The profession index.</param>
    internal void SubscribeEventsForProfession(string whichProfession)
    {
        if (!EventsByProfession.TryGetValue(whichProfession, out var events)) return;

        ModEntry.Log($"[EventSubscriber]: Subscribing to {whichProfession} profession events...", LogLevel.Trace);
        foreach (var e in events) Subscribe(e);
    }

    /// <summary>Unsubscribe the event listener from all events required by a specific profession.</summary>
    /// <param name="whichProfession">The profession index.</param>
    internal void UnsubscribeProfessionEvents(string whichProfession)
    {
        if (!EventsByProfession.TryGetValue(whichProfession, out var events)) return;

        List<IEvent> except = new();
        if (whichProfession == "Prospector" && Game1.player.HasProfession("Scavenger") ||
            whichProfession == "Scavenger" && Game1.player.HasProfession("Prospector"))
            except.Add(new TrackerButtonsChangedEvent());

        ModEntry.Log($"[EventSubscriber]: Unsubscribing from {whichProfession} profession events...",
            LogLevel.Trace);
        foreach (var e in events.Except(except)) Unsubscribe(e.GetType());
    }

    /// <summary>Subscribe the event listener to all events required for Super Mode functionality.</summary>
    internal void SubscribeSuperModeEvents()
    {
        ModEntry.Log("[EventSubscriber]: Subscribing Super Mode events...", LogLevel.Trace);
        Subscribe(
            new SuperModeButtonsChangedEvent(),
            new SuperModeGaugeRaisedAboveZeroEvent(),
            new SuperModeWarpedEvent(),
            new SuperModeModMessageReceivedEvent()
        );

        if (!Game1.currentLocation.IsCombatZone() && ModState.SuperModeGaugeValue <= 0) return;

        ModEntry.Subscriber.Subscribe(new SuperModeBarRenderingHudEvent());
        if (ModState.SuperModeGaugeValue >= ModState.SuperModeGaugeMaxValue)
            ModEntry.Subscriber.Subscribe(new SuperModeBarShakeTimerUpdateTickedEvent());
    }

    /// <summary>Unsubscribe the event listener from all events related to Super Mode functionality.</summary>
    internal void UnsubscribeSuperModeEvents()
    {
        ModEntry.Log("[EventSubscriber]: Unsubscribing Super Mode events...", LogLevel.Trace);
        UnsubscribeEventsStartingWith("SuperMode", typeof(SuperModeIndexChangedEvent));
    }

    /// <summary>Check if there are rogue events still subscribed and remove them.</summary>
    internal void CleanUpRogueEvents()
    {
        ModEntry.Log("[EventSubscriber]: Checking for rogue profession events...", LogLevel.Trace);
        foreach (var e in _subscribed.Cast<BaseEvent>()
                     .Where(e =>
                     {
                         var prefix = e.GetType().Name.SplitCamelCase().First();
                         return Utility.Professions.IndexByName.Contains(prefix) && !Game1.player.HasProfession(prefix) ||
                                prefix == "Tracker" && !Game1.player.HasAnyOfProfessions("Prospector", "Scavenger") ||
                                prefix == "SuperMode" &&
                                !Game1.player.HasAnyOfProfessions("Brute", "Poacher", "Piper", "Desperado");
                     })
                     .Reverse()) Unsubscribe(e.GetType());
        ModEntry.Log("[EventSubscriber]: Done unsubscribing rogue events.", LogLevel.Trace);
    }

    /// <summary>Whether the event listener is subscribed to the specified event type.</summary>
    /// <param name="eventType">The event type to check.</param>
    internal bool IsSubscribed(Type eventType)
    {
        return _subscribed.ContainsType(eventType);
    }

    /// <summary>Get an event instance of the specified event type.</summary>
    /// <param name="eventType">An event type.</param>
    internal IEvent Get(Type eventType)
    {
        return _subscribed.FirstOrDefault(e => e is not null && e.GetType() == eventType);
    }

    /// <summary>Try to get an event instance of the specified event type.</summary>
    /// <param name="eventType">An event type.</param>
    /// <param name="got">The matched event, if any.</param>
    /// <returns>Returns <c>True</c> if a matching event was found, or <c>False</c> otherwise.</returns>
    internal bool TryGet(Type eventType, out IEvent got)
    {
        got = Get(eventType);
        return got is not null;
    }
}