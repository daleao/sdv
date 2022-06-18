namespace DaLion.Stardew.Professions.Framework;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

using Common.Extensions;
using Common.Extensions.Reflection;
using Events;
using Events.Content;
using Events.Display;
using Events.GameLoop;
using Events.Input;
using Events.Multiplayer;
using Events.Player;
using Extensions;
using Framework.Events.Ultimate;
using Framework.Events.TreasureHunt;

#endregion using directives

/// <summary>Manages dynamic enabling and disabling of events for modded professions.</summary>
internal static class EventManager
{
    private static readonly Dictionary<Profession, List<Type>> EventsByProfession = new()
    {
        {Profession.Aquarist, new() {typeof(HostFishPondDataRequestedEvent)}},
        {Profession.Brute, new() {typeof(BruteWarpedEvent)}},
        {Profession.Conservationist, new() {typeof(HostConservationismDayEndingEvent)}},
        {Profession.Desperado, new() {typeof(DesperadoUpdateTickedEvent)}},
        {Profession.Piper, new() {typeof(PiperWarpedEvent)}},
        {Profession.Prospector, new() {typeof(ProspectorHuntDayStartedEvent), typeof(ProspectorWarpedEvent), typeof(TrackerButtonsChangedEvent)}},
        {Profession.Scavenger, new() {typeof(ScavengerHuntDayStartedEvent), typeof(ScavengerWarpedEvent), typeof(TrackerButtonsChangedEvent)}},
        {Profession.Spelunker, new() {typeof(SpelunkerWarpedEvent)}}
    };

    private static readonly List<IEvent> _ManagedEvents = new();

    /// <summary>Construct an instance.</summary>
    internal static void Init(IModEvents modEvents)
    {
        Log.D("[EventManager]: Gathering events...");
        
        // instantiate event classes
        var events = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IEvent)))
            .Where(t => t.IsAssignableTo(typeof(IEvent)) && !t.IsAbstract &&
                        t.GetConstructors(BindingFlags.Instance|BindingFlags.NonPublic).All(c => c.GetParameters().Length == 0))
            .ToList();

#if RELEASE
        events = events.Where(t => !t.Name.StartsWith("Debug")).ToList();
#endif

        Log.D($"[EventManager]: Found {events.Count} event classes. Initializing events...");
        foreach (var e in events.Select(t => (IEvent) t.RequireConstructor().Invoke(Array.Empty<object>())))
            _ManagedEvents.Add(e);

        Log.D("[EventManager]: Done. Hooking event runners...");
        
        // hook vanilla events
        foreach (var @event in _ManagedEvents.OfType<AssetRequestedEvent>())
            modEvents.Content.AssetRequested += @event.OnAssetRequested;

        foreach (var @event in _ManagedEvents.OfType<AssetsInvalidatedEvent>())
            modEvents.Content.AssetsInvalidated += @event.OnAssetsInvalidated;

        foreach (var @event in _ManagedEvents.OfType<RenderedActiveMenuEvent>())
            modEvents.Display.RenderedActiveMenu += @event.OnRenderedActiveMenu;

        foreach (var @event in _ManagedEvents.OfType<RenderedHudEvent>())
            modEvents.Display.RenderedHud += @event.OnRenderedHud;

        foreach (var @event in _ManagedEvents.OfType<RenderedWorldEvent>())
            modEvents.Display.RenderedWorld += @event.OnRenderedWorld;

        foreach (var @event in _ManagedEvents.OfType<RenderingHudEvent>())
            modEvents.Display.RenderingHud += @event.OnRenderingHud;

        foreach (var @event in _ManagedEvents.OfType<DayEndingEvent>())
            modEvents.GameLoop.DayEnding += @event.OnDayEnding;

        foreach (var @event in _ManagedEvents.OfType<DayStartedEvent>())
            modEvents.GameLoop.DayStarted += @event.OnDayStarted;

        foreach (var @event in _ManagedEvents.OfType<GameLaunchedEvent>())
            modEvents.GameLoop.GameLaunched += @event.OnGameLaunched;

        foreach (var @event in _ManagedEvents.OfType<OneSecondUpdateTickedEvent>())
            modEvents.GameLoop.OneSecondUpdateTicked += @event.OnOneSecondUpdateTicked;

        foreach (var @event in _ManagedEvents.OfType<ReturnedToTitleEvent>())
            modEvents.GameLoop.ReturnedToTitle += @event.OnReturnedToTitle;

        foreach (var @event in _ManagedEvents.OfType<SaveLoadedEvent>())
            modEvents.GameLoop.SaveLoaded += @event.OnSaveLoaded;

        foreach (var @event in _ManagedEvents.OfType<SavingEvent>())
            modEvents.GameLoop.Saving += @event.OnSaving;

        foreach (var @event in _ManagedEvents.OfType<UpdateTickedEvent>())
            modEvents.GameLoop.UpdateTicked += @event.OnUpdateTicked;

        foreach (var @event in _ManagedEvents.OfType<ButtonsChangedEvent>())
            modEvents.Input.ButtonsChanged += @event.OnButtonsChanged;

        foreach (var @event in _ManagedEvents.OfType<CursorMovedEvent>())
            modEvents.Input.CursorMoved += @event.OnCursorMoved;

        foreach (var @event in _ManagedEvents.OfType<ModMessageReceivedEvent>())
            modEvents.Multiplayer.ModMessageReceived += @event.OnModMessageReceived;

        foreach (var @event in _ManagedEvents.OfType<PeerConnectedEvent>())
            modEvents.Multiplayer.PeerConnected += @event.OnPeerConnected;

        foreach (var @event in _ManagedEvents.OfType<PeerDisconnectedEvent>())
            modEvents.Multiplayer.PeerDisconnected += @event.OnPeerDisconnected;

        foreach (var @event in _ManagedEvents.OfType<LevelChangedEvent>())
            modEvents.Player.LevelChanged += @event.OnLevelChanged;

        foreach (var @event in _ManagedEvents.OfType<WarpedEvent>())
            modEvents.Player.Warped += @event.OnWarped;

        // hook mod events
        foreach (var @event in _ManagedEvents.OfType<TreasureHuntEndedEvent>())
            TreasureHunt.TreasureHunt.Ended += @event.OnEnded;

        foreach (var @event in _ManagedEvents.OfType<TreasureHuntStartedEvent>())
            TreasureHunt.TreasureHunt.Started += @event.OnStarted;

        foreach (var @event in _ManagedEvents.OfType<UltimateActivatedEvent>())
            Ultimate.Ultimate.Activated += @event.OnActivated;

        foreach (var @event in _ManagedEvents.OfType<UltimateChargeIncreasedEvent>())
            Ultimate.Ultimate.ChargeIncreased += @event.OnChargeIncreased;

        foreach (var @event in _ManagedEvents.OfType<UltimateChargeInitiatedEvent>())
            Ultimate.Ultimate.ChargeInitiated += @event.OnChargeInitiated;

        foreach (var @event in _ManagedEvents.OfType<UltimateDeactivatedEvent>())
            Ultimate.Ultimate.Deactivated += @event.OnDeactivated;

        foreach (var @event in _ManagedEvents.OfType<UltimateEmptiedEvent>())
            Ultimate.Ultimate.Emptied += @event.OnEmptied;

        foreach (var @event in _ManagedEvents.OfType<UltimateFullyChargedEvent>())
            Ultimate.Ultimate.FullyCharged += @event.OnFullyCharged;

        Log.D("[EventManager]: Event initialization complete.");

#if DEBUG
        EnableAllStartingWith("Debug");
#endif
    }

    internal static IList<IEvent> Events => _ManagedEvents.AsReadOnly();

    /// <summary>Enable the specified <see cref="IEvent" /> types.</summary>
    /// <param name="eventTypes">A collection of <see cref="IEvent" /> types.</param>
    internal static void Enable(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IEvent)) || type.IsAbstract)
            {
                Log.W($"[EventManager]: {type.Name} is not a valid IEvent type.");
                continue;
            }

            var e = _ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.W($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            e.Enable();
            Log.D($"[EventManager]: Enabled {type.Name}.");
        }
    }

    /// <summary>Disable events from the event listener.</summary>
    /// <param name="eventTypes">The event types to be disabled.</param>
    internal static void Disable(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IEvent)) || type.IsAbstract)
            {
                Log.W($"[EventManager]: {type.Name} is not a valid IEvent type.");
                continue;
            }

            var e = _ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.W($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            e.Disable();
            Log.D($"[EventManager]: Disabled {type.Name}.");
        }
    }

    /// <summary>Enable all events required by the local player's current professions.</summary>
    internal static void EnableAllForLocalPlayer()
    {
        Log.D($"[EventManager]: Enabling profession events for farmer {Game1.player.Name}...");
        foreach (var pid in Game1.player.professions)
            try
            {
                if (Profession.TryFromValue(pid, out var profession))
                    EnableAllForProfession(profession);
            }
            catch (IndexOutOfRangeException)
            {
                Log.D($"[EventManager]: Unexpected profession index {pid} will be ignored.");
            }

        if (Context.IsMultiplayer)
        {
            Log.D("[EventManager]: Enabling multiplayer events...");
            Enable(typeof(ToggledUltimateModMessageReceivedEvent));
            if (Context.IsMainPlayer) Enable(typeof(HostPeerConnectedEvent), typeof(HostPeerDisconnectedEvent));
        }

        if (ModEntry.ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
            Enable(typeof(VerifyHudThemeWarpedEvent));

        if (ModEntry.ModHelper.ModRegistry.IsLoaded("spacechase0.SpaceCore"))
            Enable(typeof(SpaceCoreSaveLoadedEvent));


        Log.D("[EventManager]: Done enabling local player events.");
    }

    /// <summary>Disable all non-static events.</summary>
    internal static void DisableAllForLocalPlayer()
    {
        Log.D("[EventManager]: Disabling local player events...");
        var eventsToRemove = _ManagedEvents
            .Where(e => !e.GetType().Name.SplitCamelCase().First().IsIn("Static", "Debug") &&
                        !e.GetType().IsAssignableTo(typeof(SaveLoadedEvent)) &&
                        !e.GetType().IsAssignableTo(typeof(ReturnedToTitleEvent)))
            .Select(e => e.GetType())
            .ToArray();
        Disable(eventsToRemove);
        Log.D("[EventManager]: Done disabling local player events.");
    }

    /// <summary>Enable all events required by the specified profession.</summary>
    /// <param name="profession">A profession.</param>
    internal static void EnableAllForProfession(Profession profession)
    {
        if (profession == Profession.Conservationist && !Context.IsMainPlayer ||
            !EventsByProfession.TryGetValue(profession, out var events)) return;

        Log.D($"[EventManager]: Enabling events for {profession}...");
        Enable(events.ToArray());
    }

    /// <summary>Disable all events related to the specified profession.</summary>
    /// <param name="profession">A profession.</param>
    internal static void DisableAllForProfession(Profession profession)
    {
        if (profession == Profession.Conservationist && Game1.game1.DoesAnyPlayerHaveProfession(Profession.Conservationist, out _)
            || !EventsByProfession.TryGetValue(profession, out var events)) return;

        if (profession == Profession.Spelunker) events.Add(typeof(SpelunkerUpdateTickedEvent));

        List<Type> except = new();
        if (profession == Profession.Prospector && Game1.player.HasProfession(Profession.Scavenger) ||
            profession == Profession.Scavenger && Game1.player.HasProfession(Profession.Prospector))
            except.Add(typeof(TrackerButtonsChangedEvent));

        Log.D($"[EventManager]: Disabling {profession} events...");
        Disable(events.Except(except).ToArray());
    }

    /// <summary>Enable all event types starting with the specified prefix.</summary>
    /// <param name="prefix">A <see cref="string" /> prefix.</param>
    /// <param name="except">Types to be excluded, if any.</param>
    internal static void EnableAllStartingWith(string prefix, params Type[] except)
    {
        Log.D($"[EventManager]: Searching for '{prefix}' events to be enabled...");
        var toBeEnabled = _ManagedEvents
            .Select(e => e.GetType())
            .Where(t => t.Name.StartsWith(prefix))
            .Except(except)
            .ToArray();

        Log.D($"Found {toBeEnabled.Length} events. Enabling...");
        Enable(toBeEnabled);
    }

    /// <summary>Disable all event types starting with the specified prefix.</summary>
    /// <param name="prefix">A <see cref="string" /> prefix.</param>
    /// <param name="except">Types to be excluded, if any.</param>
    internal static void DisableAllStartingWith(string prefix, params Type[] except)
    {
        Log.D($"[EventManager]: Searching for '{prefix}' events to be disabled...");
        var toBeDisabled = _ManagedEvents
            .Select(e => e.GetType())
            .Where(t => t.Name.StartsWith(prefix))
            .Except(except)
            .ToArray();

        Log.D($"Found {toBeDisabled.Length} events. Disabling...");
        Disable(toBeDisabled);
    }

    /// <summary>Get an event instance of the specified event type.</summary>
    /// <typeparam name="T">A type implementing <see cref="IEvent"/>.</typeparam>
    internal static T Get<T>() where T : IEvent
    {
        return _ManagedEvents.OfType<T>().FirstOrDefault();
    }

    /// <summary>Try to get an event instance of the specified event type.</summary>
    /// <param name="got">The matched event, if any.</param>
    /// <typeparam name="T">A type implementing <see cref="IEvent"/>.</typeparam>
    /// <returns>Returns <c>True</c> if a matching event was found, or <c>False</c> otherwise.</returns>
    internal static bool TryGet<T>(out T got) where T : IEvent
    {
        got = Get<T>();
        return got is not null;
    }

    /// <summary>Check if the specified event type is enabled.</summary>
    /// <typeparam name="T">A type implementing <see cref="IEvent"/>.</typeparam>
    internal static bool IsEnabled<T>() where T : IEvent
    {
        return TryGet<T>(out var got) && got.IsEnabled;
    }

    /// <summary>Enumerate all currently enabled events.</summary>
    internal static IEnumerable<IEvent> GetAllEnabled()
    {
        return _ManagedEvents.Where(e => e.IsEnabled);
    }

    /// <summary>Enumerate all currently enabled events.</summary>
    internal static IEnumerable<IEvent> GetAllEnabledForScreen(int screenId)
    {
        return _ManagedEvents.Where(e => e.IsEnabledForScreen(screenId));
    }

    /// <summary>Add a new <see cref="IEvent"/> instance to the list of managed events.</summary>
    /// <param name="event">An <see cref="IEvent"/> instance.</param>
    internal static void Manage(IEvent @event)
    {
        _ManagedEvents.Add(@event);
    }
}