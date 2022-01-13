using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using DaLion.Stardew.Common.Extensions;
using DaLion.Stardew.Common.Harmony;
using DaLion.Stardew.Professions.Framework.Events;
using DaLion.Stardew.Professions.Framework.Events.Display;
using DaLion.Stardew.Professions.Framework.Events.GameLoop;
using DaLion.Stardew.Professions.Framework.Events.Input;
using DaLion.Stardew.Professions.Framework.Events.Multiplayer;
using DaLion.Stardew.Professions.Framework.Events.Player;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework;

/// <summary>Manages dynamic subscribing and unsubscribing of events for modded professions.</summary>
internal class EventManager
{
    private static readonly Dictionary<string, List<IEvent>> EventsByProfession = new()
    {
        {"Conservationist", new() {new GlobalConservationistDayEndingEvent()}},
        {"Poacher", new() {new PoacherWarpedEvent()}},
        {"Prospector", new() {new ProspectorHuntDayStartedEvent(), new ProspectorWarpedEvent(), new TrackerButtonsChangedEvent()}},
        {"Scavenger", new() {new ScavengerHuntDayStartedEvent(), new ScavengerWarpedEvent(), new TrackerButtonsChangedEvent()}},
        {"Spelunker", new() {new SpelunkerWarpedEvent()}}
    };

    private readonly List<IEvent> _events = new();

    /// <summary>Construct an instance.</summary>
    internal EventManager(IModEvents events)
    {
        // initialize event classes
        InitAll();

        // hook SMAPI event runners
        events.Display.RenderedActiveMenu += RunRenderedActiveMenuEvents;
        events.Display.RenderedHud += RunRenderedHudEvents;
        events.Display.RenderedWorld += RunRenderedWorldEvents;
        events.Display.RenderingHud += RunRenderingHudEvents;

        events.GameLoop.DayEnding += RunDayEndingEvents;
        events.GameLoop.DayStarted += RunDayStartedEvents;
        events.GameLoop.GameLaunched += RunGameLaunchedEvents;
        events.GameLoop.ReturnedToTitle += RunReturnedToTitleEvents;
        events.GameLoop.SaveLoaded += RunSaveLoadedEvents;
        events.GameLoop.UpdateTicked += RunUpdateTickedEvents;

        events.Input.ButtonsChanged += RunButtonsChangedEvents;
        events.Input.CursorMoved += RunCursorMovedEvents;

        events.Multiplayer.ModMessageReceived += RunModMessageReceivedEvents;
        events.Multiplayer.PeerConnected += RunPeerConnectedEvents;

        events.Player.LevelChanged += RunLevelChangedEvents;
        events.Player.Warped += RunWarpedEvents;

        // enable debug if necessary
        if (ModEntry.Config.EnableDebug) EnableAllDebug();
    }

    internal IList<IEvent> Events => _events.AsReadOnly();

    /// <summary>Instantiate one of every <see cref="IEvent" /> class in the assembly using reflection.</summary>
    internal void InitAll()
    {
        ModEntry.Log("[EventManager]: Gathering events...", ModEntry.DefaultLogLevel);
        var events = AccessTools.GetTypesFromAssembly(Assembly.GetAssembly(typeof(IEvent)))
            .Where(t => t.IsAssignableTo(typeof(IEvent)) && !t.IsAbstract).ToList();
        if (!ModEntry.Config.EnableDebug) events = events.Where(t => !t.Name.StartsWith("Debug")).ToList();

        ModEntry.Log($"[EventManager]: Found {events.Count} event classes. Initializing events...", ModEntry.DefaultLogLevel);
        foreach (var e in events.Select(t => (IEvent) t.Constructor().Invoke(Array.Empty<object>())))
            _events.Add(e);

        ModEntry.Log("[EventManager]: Done.", ModEntry.DefaultLogLevel);
    }

    /// <summary>Enable the specified <see cref="IEvent" /> types.</summary>
    /// <param name="eventTypes">A collection of <see cref="IEvent" /> types.</param>
    internal void Enable(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IEvent)) || type.IsAbstract)
            {
                ModEntry.Log($"[EventManager]: {type.Name} is not a valid IEvent type.", LogLevel.Warn);
                continue;
            }

            var e = _events.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                ModEntry.Log($"[EventManager]: The type {type.Name} was not found.", LogLevel.Warn);
                continue;
            }

            e.Enable();
            ModEntry.Log($"[EventManager]: Hooked {type.Name}.", ModEntry.DefaultLogLevel);
        }
    }

    /// <summary>Disable events from the event listener.</summary>
    /// <param name="eventTypes">The event types to be unsubscribed.</param>
    internal void Disable(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IEvent)) || type.IsAbstract)
            {
                ModEntry.Log($"[EventManager]: {type.Name} is not a valid IEvent type.", LogLevel.Warn);
                continue;
            }

            var e = _events.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                ModEntry.Log($"[EventManager]: The type {type.Name} was not found.", LogLevel.Warn);
                continue;
            }

            e.Disable();
            ModEntry.Log($"[EventManager]: Unhooked {type.Name}.", ModEntry.DefaultLogLevel);
        }
    }

    /// <summary>Enable all events required by the local player's current professions.</summary>
    internal void EnableAllForLocalPlayer()
    {
        ModEntry.Log($"[EventManager]: Searching for events required by farmer {Game1.player.Name}...",
            ModEntry.DefaultLogLevel);
        foreach (var professionIndex in Game1.player.professions)
            try
            {
                EnableAllForProfession(Utility.Professions.NameOf(professionIndex));
            }
            catch (IndexOutOfRangeException)
            {
                ModEntry.Log($"[EventManager]: Unexpected profession index {professionIndex} will be ignored.",
                    ModEntry.DefaultLogLevel);
            }

        if (Context.IsMainPlayer)
            Enable(typeof(HostPeerConnectedEvent), typeof(HostPeerDisconnectedEvent));
        else if (Context.IsMultiplayer)
            Enable(typeof(ToggledSuperModeModMessageReceivedEvent));
        ModEntry.Log("[EventManager]: Done subscribing local player events.", ModEntry.DefaultLogLevel);
    }

    /// <summary>Disable all non-static events.</summary>
    internal void DisableAllForLocalPlayer()
    {
        ModEntry.Log("[EventManager]:  local player events...", ModEntry.DefaultLogLevel);
        var eventsToRemove = _events
            .Where(e => !e.GetType().Name.SplitCamelCase().First().IsAnyOf("Static", "Debug"))
            .Select(subscribed => subscribed.GetType())
            .ToArray();
        Disable(eventsToRemove);
        ModEntry.Log("[EventManager]: Done unsubscribing local player events.", ModEntry.DefaultLogLevel);
    }

    /// <summary>Enable all events required by the specified profession.</summary>
    /// <param name="professionName">A profession name.</param>
    internal void EnableAllForProfession(string professionName)
    {
        if (professionName == "Conservationist" && !Context.IsMainPlayer ||
            !EventsByProfession.TryGetValue(professionName, out var events)) return;

        ModEntry.Log($"[EventManager]: Enabling events for {professionName}...", ModEntry.DefaultLogLevel);
        Enable(events.Select(e => e.GetType()).ToArray());
    }

    /// <summary>Disable all events related to the specified profession.</summary>
    /// <param name="professionName">A profession name.</param>
    internal void DisableAllForProfession(string professionName)
    {
        if (!EventsByProfession.TryGetValue(professionName, out var events)) return;

        List<IEvent> except = new();
        if (professionName == "Prospector" && Game1.player.HasProfession("Scavenger") ||
            professionName == "Scavenger" && Game1.player.HasProfession("Prospector"))
            except.Add(new TrackerButtonsChangedEvent());

        ModEntry.Log($"[EventManager]: Disabling {professionName} events...",
            ModEntry.DefaultLogLevel);
        Disable(events.Except(except).Select(e => e.GetType()).ToArray());
    }

    /// <summary>Enable all events required for Super Mode functionality.</summary>
    internal void EnableAllForSuperMode()
    {
        ModEntry.Log("[EventManager]: Enabling base Super Mode events...", ModEntry.DefaultLogLevel);
        Enable(typeof(SuperModeWarpedEvent));

        if (!Game1.currentLocation.IsCombatZone() || !ModEntry.Config.EnableSuperMode) return;

        Enable(typeof(SuperModeGaugeRenderingHudEvent));
    }

    /// <summary>Disable all events related to Super Mode functionality.</summary>
    internal void DisableAllForSuperMode()
    {
        ModEntry.Log("[EventManager]: Unsubscribing Super Mode events...", ModEntry.DefaultLogLevel);
        DisableAllStartingWith("SuperMode");
    }

    /// <summary>Enable all debug events.</summary>
    internal void EnableAllDebug()
    {
        ModEntry.Log("[EventManager]: Enabling debug events...", LogLevel.Debug);
        EnableAllStartingWith("Debug");
    }

    /// <summary>Disable all debug events.</summary>
    internal void DisableAllDebug()
    {
        ModEntry.Log("[EventManager]: Disabling debug events...", LogLevel.Debug);
        DisableAllStartingWith("Debug", typeof(DebugButtonsChangedEvent));
    }

    /// <summary>Get an event instance of the specified event type.</summary>
    /// <typeparam name="T">A type implementing <see cref="IEvent"/>.</typeparam>
    internal T Get<T>() where T : IEvent
    {
        return _events.OfType<T>().FirstOrDefault();
    }

    /// <summary>Try to get an event instance of the specified event type.</summary>
    /// <param name="got">The matched event, if any.</param>
    /// <typeparam name="T">A type implementing <see cref="IEvent"/>.</typeparam>
    /// <returns>Returns <c>True</c> if a matching event was found, or <c>False</c> otherwise.</returns>
    internal bool TryGet<T>(out T got) where T : IEvent
    {
        got = Get<T>();
        return got is not null;
    }

    /// <summary>Enumerate all currently enabled events.</summary>
    internal IEnumerable<IEvent> GetAllEnabled()
    {
        return _events.Cast<BaseEvent>().Where(e => e.IsEnabled);
    }

    /// <summary>Enumerate all currently enabled events.</summary>
    internal IEnumerable<IEvent> GetAllEnabledForScreen(int screenId)
    {
        return _events.Cast<BaseEvent>().Where(e => e.IsEnabledForScreen(screenId));
    }

    #region event runners

    // display events
    private void RunRenderedActiveMenuEvents(object sender, RenderedActiveMenuEventArgs e)
    {
        foreach (var renderedActiveMenuEvent in _events.OfType<RenderedActiveMenuEvent>())
            renderedActiveMenuEvent.OnRenderedActiveMenu(sender, e);
    }

    private void RunRenderedHudEvents(object sender, RenderedHudEventArgs e)
    {
        foreach (var renderedHudEvent in _events.OfType<RenderedHudEvent>())
            renderedHudEvent.OnRenderedHud(sender, e);
    }

    private void RunRenderedWorldEvents(object sender, RenderedWorldEventArgs e)
    {
        foreach (var renderedWorldEvent in _events.OfType<RenderedWorldEvent>())
            renderedWorldEvent.OnRenderedWorld(sender, e);
    }

    private void RunRenderingHudEvents(object sender, RenderingHudEventArgs e)
    {
        foreach (var renderingHudEvent in _events.OfType<RenderingHudEvent>())
            renderingHudEvent.OnRenderingHud(sender, e);
    }

    // game loop events
    private void RunDayEndingEvents(object sender, DayEndingEventArgs e)
    {
        foreach (var dayEndingEvent in _events.OfType<DayEndingEvent>())
            dayEndingEvent.OnDayEnding(sender, e);
    }

    private void RunDayStartedEvents(object sender, DayStartedEventArgs e)
    {
        foreach (var dayStartedEvent in _events.OfType<DayStartedEvent>())
            dayStartedEvent.OnDayStarted(sender, e);
    }

    private void RunGameLaunchedEvents(object sender, GameLaunchedEventArgs e)
    {
        foreach (var gameLaunchedEvent in _events.OfType<GameLaunchedEvent>())
            gameLaunchedEvent.OnGameLaunched(sender, e);
    }

    private void RunReturnedToTitleEvents(object sender, ReturnedToTitleEventArgs e)
    {
        foreach (var returnedToTitleEvent in _events.OfType<ReturnedToTitleEvent>())
            returnedToTitleEvent.OnReturnedToTitle(sender, e);
    }

    private void RunSaveLoadedEvents(object sender, SaveLoadedEventArgs e)
    {
        foreach (var saveLoadedEvent in _events.OfType<SaveLoadedEvent>())
            saveLoadedEvent.OnSaveLoaded(sender, e);
    }

    private void RunUpdateTickedEvents(object sender, UpdateTickedEventArgs e)
    {
        foreach (var updateTickedEvent in _events.OfType<UpdateTickedEvent>())
            updateTickedEvent.OnUpdateTicked(sender, e);
    }

    // input events
    private void RunButtonsChangedEvents(object sender, ButtonsChangedEventArgs e)
    {
        foreach (var buttonsChangedEvent in _events.OfType<ButtonsChangedEvent>())
            buttonsChangedEvent.OnButtonsChanged(sender, e);
    }

    private void RunCursorMovedEvents(object sender, CursorMovedEventArgs e)
    {
        foreach (var cursorMovedEvent in _events.OfType<CursorMovedEvent>())
            cursorMovedEvent.OnCursorMoved(sender, e);
    }

    // multiplayer events
    private void RunModMessageReceivedEvents(object sender, ModMessageReceivedEventArgs e)
    {
        foreach (var modMessageReceivedEvent in _events.OfType<ModMessageReceivedEvent>())
            modMessageReceivedEvent.OnModMessageReceived(sender, e);
    }

    private void RunPeerConnectedEvents(object sender, PeerConnectedEventArgs e)
    {
        foreach (var peerConnectedEvent in _events.OfType<PeerConnectedEvent>())
            peerConnectedEvent.OnPeerConnected(sender, e);
    }

    // player events
    private void RunLevelChangedEvents(object sender, LevelChangedEventArgs e)
    {
        foreach (var levelChangedEvent in _events.OfType<LevelChangedEvent>())
            levelChangedEvent.OnLevelChanged(sender, e);
    }

    private void RunWarpedEvents(object sender, WarpedEventArgs e)
    {
        foreach (var warpedEvent in _events.OfType<WarpedEvent>())
            warpedEvent.OnWarped(sender, e);
    }

    #endregion event runners

    #region private methods

    /// <summary>Enable all event types starting with the specified prefix.</summary>
    /// <param name="prefix">A <see cref="string" /> prefix.</param>
    /// <param name="except">Types to be excluded, if any.</param>
    private void EnableAllStartingWith(string prefix, params Type[] except)
    {
        ModEntry.Log($"[EventManager]: Searching for '{prefix}' events to be enabled...", ModEntry.DefaultLogLevel);
        var toBeHooked = _events
            .Select(e => e.GetType())
            .Where(t => t.Name.StartsWith(prefix))
            .Except(except)
            .ToArray();

        ModEntry.Log($"Found {toBeHooked.Length} events. Hooking...", ModEntry.DefaultLogLevel);
        Enable(toBeHooked);
    }

    /// <summary>Disable all event types starting with the specified prefix.</summary>
    /// <param name="prefix">A <see cref="string" /> prefix.</param>
    /// <param name="except">Types to be excluded, if any.</param>
    private void DisableAllStartingWith(string prefix, params Type[] except)
    {
        ModEntry.Log($"[EventManager]: Searching for '{prefix}' events to be unhooked...", ModEntry.DefaultLogLevel);
        var toBeUnhooked = _events
            .Select(e => e.GetType())
            .Where(t => t.Name.StartsWith(prefix))
            .Except(except)
            .ToArray();

        ModEntry.Log($"Found {toBeUnhooked.Length} events. Unhooking...", ModEntry.DefaultLogLevel);
        Disable(toBeUnhooked);
    }

    #endregion private methods
}