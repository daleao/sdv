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
using Events;
using Events.Display;
using Events.GameLoop;
using Events.Input;
using Events.Multiplayer;
using Events.Player;
using Extensions;

#endregion using directives

/// <summary>Manages dynamic enabling and disabling of events for modded professions.</summary>
internal static class EventManager
{
    private static readonly Dictionary<Profession, List<Type>> EventsByProfession = new()
    {
        {Profession.Brute, new() {typeof(BruteUpdateTickedEvent)}},
        {Profession.Conservationist, new() {typeof(HostConservationismDayEndingEvent)}},
        {Profession.Piper, new() {typeof(PiperWarpedEvent)}},
        {Profession.Prospector, new() {typeof(ProspectorHuntDayStartedEvent), typeof(ProspectorWarpedEvent), typeof(TrackerButtonsChangedEvent)}},
        {Profession.Scavenger, new() {typeof(ScavengerHuntDayStartedEvent), typeof(ScavengerWarpedEvent), typeof(TrackerButtonsChangedEvent)}},
        {Profession.Spelunker, new() {typeof(SpelunkerWarpedEvent)}},
        {Profession.Desperado, new() {typeof(DesperadoUpdateTickedEvent)}}
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
        foreach (var e in events.Select(t => (IEvent)t.Constructor().Invoke(Array.Empty<object>())))
            _events.Add(e);

        Log.D("[EventManager]: Done. Hooking event runners...");
        
        // hook event runners
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

    // display events
    private static void RunRenderedActiveMenuEvents(object sender, RenderedActiveMenuEventArgs e)
    {
        foreach (var renderedActiveMenuEvent in _events.OfType<RenderedActiveMenuEvent>())
            renderedActiveMenuEvent.OnRenderedActiveMenu(sender, e);
    }

    private static void RunRenderedHudEvents(object sender, RenderedHudEventArgs e)
    {
        foreach (var renderedHudEvent in _events.OfType<RenderedHudEvent>())
            renderedHudEvent.OnRenderedHud(sender, e);
    }

    private static void RunRenderedWorldEvents(object sender, RenderedWorldEventArgs e)
    {
        foreach (var renderedWorldEvent in _events.OfType<RenderedWorldEvent>())
            renderedWorldEvent.OnRenderedWorld(sender, e);
    }

    private static void RunRenderingHudEvents(object sender, RenderingHudEventArgs e)
    {
        foreach (var renderingHudEvent in _events.OfType<RenderingHudEvent>())
            renderingHudEvent.OnRenderingHud(sender, e);
    }

    // game loop events
    private static void RunDayEndingEvents(object sender, DayEndingEventArgs e)
    {
        foreach (var dayEndingEvent in _events.OfType<DayEndingEvent>())
            dayEndingEvent.OnDayEnding(sender, e);
    }

    private static void RunDayStartedEvents(object sender, DayStartedEventArgs e)
    {
        foreach (var dayStartedEvent in _events.OfType<DayStartedEvent>())
            dayStartedEvent.OnDayStarted(sender, e);
    }

    private static void RunGameLaunchedEvents(object sender, GameLaunchedEventArgs e)
    {
        foreach (var gameLaunchedEvent in _events.OfType<GameLaunchedEvent>())
            gameLaunchedEvent.OnGameLaunched(sender, e);
    }

    private static void RunReturnedToTitleEvents(object sender, ReturnedToTitleEventArgs e)
    {
        foreach (var returnedToTitleEvent in _events.OfType<ReturnedToTitleEvent>())
            returnedToTitleEvent.OnReturnedToTitle(sender, e);
    }

    private static void RunSaveLoadedEvents(object sender, SaveLoadedEventArgs e)
    {
        foreach (var saveLoadedEvent in _events.OfType<SaveLoadedEvent>())
            saveLoadedEvent.OnSaveLoaded(sender, e);
    }

    private static void RunSavingEvents(object sender, SavingEventArgs e)
    {
        foreach (var savingEvent in _events.OfType<SavingEvent>())
            savingEvent.OnSaving(sender, e);
    }

    private static void RunUpdateTickedEvents(object sender, UpdateTickedEventArgs e)
    {
        foreach (var updateTickedEvent in _events.OfType<UpdateTickedEvent>())
            updateTickedEvent.OnUpdateTicked(sender, e);
    }

    // input events
    private static void RunButtonsChangedEvents(object sender, ButtonsChangedEventArgs e)
    {
        foreach (var buttonsChangedEvent in _events.OfType<ButtonsChangedEvent>())
            buttonsChangedEvent.OnButtonsChanged(sender, e);
    }

    private static void RunCursorMovedEvents(object sender, CursorMovedEventArgs e)
    {
        foreach (var cursorMovedEvent in _events.OfType<CursorMovedEvent>())
            cursorMovedEvent.OnCursorMoved(sender, e);
    }

    // multiplayer events
    private static void RunModMessageReceivedEvents(object sender, ModMessageReceivedEventArgs e)
    {
        foreach (var modMessageReceivedEvent in _events.OfType<ModMessageReceivedEvent>())
            modMessageReceivedEvent.OnModMessageReceived(sender, e);
    }

    private static void RunPeerConnectedEvents(object sender, PeerConnectedEventArgs e)
    {
        foreach (var peerConnectedEvent in _events.OfType<PeerConnectedEvent>())
            peerConnectedEvent.OnPeerConnected(sender, e);
    }

    private static void RunPeerDisconnectedEvents(object sender, PeerDisconnectedEventArgs e)
    {
        foreach (var peerDisconnectedEvent in _events.OfType<PeerDisconnectedEvent>())
            peerDisconnectedEvent.OnPeerDisconnected(sender, e);
    }

    // player events
    private static void RunLevelChangedEvents(object sender, LevelChangedEventArgs e)
    {
        foreach (var levelChangedEvent in _events.OfType<LevelChangedEvent>())
            levelChangedEvent.OnLevelChanged(sender, e);
    }

    private static void RunWarpedEvents(object sender, WarpedEventArgs e)
    {
        foreach (var warpedEvent in _events.OfType<WarpedEvent>())
            warpedEvent.OnWarped(sender, e);
    }

    #endregion event runners
}