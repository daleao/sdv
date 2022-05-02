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

    private static readonly List<IEvent> _events = new();

    /// <summary>Construct an instance.</summary>
    internal static void Init(IModEvents modEvents)
    {
        Log.D("[EventManager]: Gathering events...");
        
        // instantiate event classes
        var events = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IEvent)))
            .Where(t => t.IsAssignableTo(typeof(IEvent)) && !t.IsAbstract)
            .ToList();

#if RELEASE
        events = events.Where(t => !t.Name.StartsWith("Debug")).ToList();
#endif

        Log.D($"[EventManager]: Found {events.Count} event classes. Initializing events...");
        foreach (var e in events.Select(t => (IEvent)t.RequireConstructor().Invoke(Array.Empty<object>())))
            _events.Add(e);

        Log.D("[EventManager]: Done. Hooking event runners...");
        
        // hook event runners
        Ultimate.Ultimate.Activated += RunUltimateActivatedEvents;
        Ultimate.Ultimate.Deactivated += RunUltimateDectivatedEvents;
        Ultimate.Ultimate.ChargeInitiated += RunUltimateChargeInitiatedEvents;
        Ultimate.Ultimate.ChargeGained += RunUltimateChargeGainedEvents;
        Ultimate.Ultimate.Emptied += RunUltimateEmptiedEvents;

        modEvents.Content.AssetRequested += RunAssetRequestedEvents;
        modEvents.Display.RenderedActiveMenu += RunRenderedActiveMenuEvents;
        modEvents.Display.RenderedHud += RunRenderedHudEvents;
        modEvents.Display.RenderedWorld += RunRenderedWorldEvents;
        modEvents.Display.RenderingHud += RunRenderingHudEvents;
        modEvents.GameLoop.DayEnding += RunDayEndingEvents;
        modEvents.GameLoop.DayStarted += RunDayStartedEvents;
        modEvents.GameLoop.GameLaunched += RunGameLaunchedEvents;
        modEvents.GameLoop.ReturnedToTitle += RunReturnedToTitleEvents;
        modEvents.GameLoop.SaveLoaded += RunSaveLoadedEvents;
        modEvents.GameLoop.Saving += RunSavingEvents;
        modEvents.GameLoop.UpdateTicked += RunUpdateTickedEvents;
        modEvents.Input.ButtonsChanged += RunButtonsChangedEvents;
        modEvents.Input.CursorMoved += RunCursorMovedEvents;
        modEvents.Multiplayer.ModMessageReceived += RunModMessageReceivedEvents;
        modEvents.Multiplayer.PeerConnected += RunPeerConnectedEvents;
        modEvents.Multiplayer.PeerDisconnected += RunPeerDisconnectedEvents;
        modEvents.Player.LevelChanged += RunLevelChangedEvents;
        modEvents.Player.Warped += RunWarpedEvents;

        Log.D("[EventManager]: Event initialization complete.");

#if DEBUG
        EnableAllStartingWith("Debug");
#endif
    }

    internal static IList<IEvent> Events => _events.AsReadOnly();

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

            var e = _events.FirstOrDefault(e => e.GetType() == type);
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

            var e = _events.FirstOrDefault(e => e.GetType() == type);
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
        Log.D($"[EventManager]: Searching for events required by farmer {Game1.player.Name}...");
        foreach (var professionIndex in Game1.player.professions)
            try
            {
                EnableAllForProfession((Profession) professionIndex);
            }
            catch (IndexOutOfRangeException)
            {
                Log.D($"[EventManager]: Unexpected profession index {professionIndex} will be ignored.");
            }

        Log.D("[EventManager] Enabling other events...");
        if (Context.IsMultiplayer)
        {
            Enable(typeof(ToggledUltimateModMessageReceivedEvent));
            if (Context.IsMainPlayer)
               Enable(typeof(HostPeerConnectedEvent), typeof(HostPeerDisconnectedEvent));
        }

        if (ModEntry.ModHelper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
            Enable(typeof(VerifyHudThemeWarpedEvent));

        Log.D("[EventManager]: Done enabling local player events.");
    }

    /// <summary>Disable all non-static events.</summary>
    internal static void DisableAllForLocalPlayer()
    {
        Log.D("[EventManager]:  local player events...");
        var eventsToRemove = _events
            .Where(e => !e.GetType().Name.SplitCamelCase().First().IsAnyOf("Static", "Debug"))
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
        var toBeEnabled = _events
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
        var toBeDisabled = _events
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
        return _events.OfType<T>().FirstOrDefault();
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
        return _events.Cast<BaseEvent>().Where(e => e.IsEnabled);
    }

    /// <summary>Enumerate all currently enabled events.</summary>
    internal static IEnumerable<IEvent> GetAllEnabledForScreen(int screenId)
    {
        return _events.Cast<BaseEvent>().Where(e => e.IsEnabledForScreen(screenId));
    }

    #region event runners

    // ultimate events
    private static void RunUltimateActivatedEvents(object sender, UltimateActivatedEventArgs e)
    {
        foreach (var @event in _events.OfType<UltimateActivatedEvent>())
            @event.OnUltimateActivated(sender, e);
    }

    private static void RunUltimateDectivatedEvents(object sender, UltimateDeactivatedEventArgs e)
    {
        foreach (var @event in _events.OfType<UltimateDeactivatedEvent>())
            @event.OnUltimateDeactivated(sender, e);
    }

    private static void RunUltimateChargeInitiatedEvents(object sender, UltimateChargeInitiatedEventArgs e)
    {
        foreach (var @event in _events.OfType<UltimateChargeInitiatedEvent>())
            @event.OnUltimateChargeInitiated(sender, e);
    }

    private static void RunUltimateChargeGainedEvents(object sender, UltimateChargeGainedEventArgs e)
    {
        foreach (var @event in _events.OfType<UltimateChargeGainedEvent>())
            @event.OnUltimateChargeGained(sender, e);
    }

    private static void RunUltimateEmptiedEvents(object sender, UltimateEmptiedEventArgs e)
    {
        foreach (var @event in _events.OfType<UltimateEmptiedEvent>())
            @event.OnUltimateEmptied(sender, e);
    }

    // content events
    private static void RunAssetRequestedEvents(object sender, AssetRequestedEventArgs e)
    {
        foreach (var @event in _events.OfType<AssetRequestedEvent>())
            @event.OnAssetRequested(sender, e);
    }

    // display events
    private static void RunRenderedActiveMenuEvents(object sender, RenderedActiveMenuEventArgs e)
    {
        foreach (var @event in _events.OfType<RenderedActiveMenuEvent>())
            @event.OnRenderedActiveMenu(sender, e);
    }

    private static void RunRenderedHudEvents(object sender, RenderedHudEventArgs e)
    {
        foreach (var @event in _events.OfType<RenderedHudEvent>())
            @event.OnRenderedHud(sender, e);
    }

    private static void RunRenderedWorldEvents(object sender, RenderedWorldEventArgs e)
    {
        foreach (var @event in _events.OfType<RenderedWorldEvent>())
            @event.OnRenderedWorld(sender, e);
    }

    private static void RunRenderingHudEvents(object sender, RenderingHudEventArgs e)
    {
        foreach (var @event in _events.OfType<RenderingHudEvent>())
            @event.OnRenderingHud(sender, e);
    }

    // game loop events
    private static void RunDayEndingEvents(object sender, DayEndingEventArgs e)
    {
        foreach (var @event in _events.OfType<DayEndingEvent>())
            @event.OnDayEnding(sender, e);
    }

    private static void RunDayStartedEvents(object sender, DayStartedEventArgs e)
    {
        foreach (var @event in _events.OfType<DayStartedEvent>())
            @event.OnDayStarted(sender, e);
    }

    private static void RunGameLaunchedEvents(object sender, GameLaunchedEventArgs e)
    {
        foreach (var @event in _events.OfType<GameLaunchedEvent>())
            @event.OnGameLaunched(sender, e);
    }

    private static void RunReturnedToTitleEvents(object sender, ReturnedToTitleEventArgs e)
    {
        foreach (var @event in _events.OfType<ReturnedToTitleEvent>())
            @event.OnReturnedToTitle(sender, e);
    }

    private static void RunSaveLoadedEvents(object sender, SaveLoadedEventArgs e)
    {
        foreach (var @event in _events.OfType<SaveLoadedEvent>())
            @event.OnSaveLoaded(sender, e);
    }

    private static void RunSavingEvents(object sender, SavingEventArgs e)
    {
        foreach (var @event in _events.OfType<SavingEvent>())
            @event.OnSaving(sender, e);
    }

    private static void RunUpdateTickedEvents(object sender, UpdateTickedEventArgs e)
    {
        foreach (var @event in _events.OfType<UpdateTickedEvent>())
            @event.OnUpdateTicked(sender, e);
    }

    // input events
    private static void RunButtonsChangedEvents(object sender, ButtonsChangedEventArgs e)
    {
        foreach (var @event in _events.OfType<ButtonsChangedEvent>())
            @event.OnButtonsChanged(sender, e);
    }

    private static void RunCursorMovedEvents(object sender, CursorMovedEventArgs e)
    {
        foreach (var @event in _events.OfType<CursorMovedEvent>())
            @event.OnCursorMoved(sender, e);
    }

    // multiplayer events
    private static void RunModMessageReceivedEvents(object sender, ModMessageReceivedEventArgs e)
    {
        foreach (var @event in _events.OfType<ModMessageReceivedEvent>())
            @event.OnModMessageReceived(sender, e);
    }

    private static void RunPeerConnectedEvents(object sender, PeerConnectedEventArgs e)
    {
        foreach (var @event in _events.OfType<PeerConnectedEvent>())
            @event.OnPeerConnected(sender, e);
    }

    private static void RunPeerDisconnectedEvents(object sender, PeerDisconnectedEventArgs e)
    {
        foreach (var @event in _events.OfType<PeerDisconnectedEvent>())
            @event.OnPeerDisconnected(sender, e);
    }

    // player events
    private static void RunLevelChangedEvents(object sender, LevelChangedEventArgs e)
    {
        foreach (var @event in _events.OfType<LevelChangedEvent>())
            @event.OnLevelChanged(sender, e);
    }

    private static void RunWarpedEvents(object sender, WarpedEventArgs e)
    {
        foreach (var @event in _events.OfType<WarpedEvent>())
            @event.OnWarped(sender, e);
    }

    #endregion event runners
}