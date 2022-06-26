namespace DaLion.Common.Events;

#region using directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using StardewModdingAPI.Events;

using Extensions.Reflection;

#endregion using directives

/// <summary>Instantiates and manages dynamic hooking and unhooking of <see cref="ManagedEvent"/> classes in the assembly.</summary>
internal class EventManager
{
    /// <summary>Cache of managed <see cref="IManagedEvent"/> instances.</summary>
    protected readonly HashSet<IManagedEvent> ManagedEvents = new();
    
    /// <inheritdoc cref="IModEvents"/>
    protected readonly IModEvents ModEvents;

    /// <summary>Construct an instance.</summary>
    /// <param name="modEvents">Manages access to events raised by SMAPI.</param>
    internal EventManager(IModEvents modEvents)
    {
        ModEvents = modEvents;
        
        Log.D("[EventManager]: Gathering events...");
        var eventTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract &&
                         // event classes may or not have the required internal parameterized constructor accepting only the manager instance, depending on whether they are SMAPI or mod-handled
                         // we only want to construct SMAPI events at this point, so we filter out the rest
                        t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {GetType()},
                            null) is not null)
            .ToArray();

#if RELEASE
        eventTypes = eventTypes.Where(t => !t.Name.StartsWith("Debug")).ToArray();
#endif

        Log.D($"[EventManager]: Found {eventTypes.Length} event classes. Initializing events...");
        foreach (var e in eventTypes)
        {
            try
            {
                var @event = (IManagedEvent) e
                    .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] {GetType()}, null)!
                    .Invoke(new object?[] {this});
                ManagedEvents.Add(@event);
                Log.D($"[EventManager]: Managing {@event.GetType().Name}");
            }
            catch (Exception ex)
            {
                Log.E($"[EventManager]: Failed to manage {e.Name}.\n{ex}");
            }
        }

        Log.D("[EventManager]: Hooking to SMAPI...");

        #region hookers

        // content
        foreach (var @event in ManagedEvents.OfType<AssetReadyEvent>())
            modEvents.Content.AssetReady += @event.OnAssetReady;

        foreach (var @event in ManagedEvents.OfType<AssetRequestedEvent>())
            modEvents.Content.AssetRequested += @event.OnAssetRequested;

        foreach (var @event in ManagedEvents.OfType<AssetsInvalidatedEvent>())
            modEvents.Content.AssetsInvalidated += @event.OnAssetsInvalidated;

        foreach (var @event in ManagedEvents.OfType<LocaleChangedEvent>())
            modEvents.Content.LocaleChanged += @event.OnLocaleChanged;

        // display
        foreach (var @event in ManagedEvents.OfType<MenuChangedEvent>())
            modEvents.Display.MenuChanged += @event.OnMenuChanged;

        foreach (var @event in ManagedEvents.OfType<RenderedActiveMenuEvent>())
            modEvents.Display.RenderedActiveMenu += @event.OnRenderedActiveMenu;

        foreach (var @event in ManagedEvents.OfType<RenderedHudEvent>())
            modEvents.Display.RenderedHud += @event.OnRenderedHud;

        foreach (var @event in ManagedEvents.OfType<RenderedWorldEvent>())
            modEvents.Display.RenderedWorld += @event.OnRenderedWorld;

        foreach (var @event in ManagedEvents.OfType<RenderingEvent>())
            modEvents.Display.Rendering += @event.OnRendering;

        foreach (var @event in ManagedEvents.OfType<RenderingHudEvent>())
            modEvents.Display.RenderingHud += @event.OnRenderingHud;

        foreach (var @event in ManagedEvents.OfType<RenderingWorldEvent>())
            modEvents.Display.RenderingWorld += @event.OnRenderingWorld;

        foreach (var @event in ManagedEvents.OfType<WindowResizedEvent>())
            modEvents.Display.WindowResized += @event.OnWindowResized;
        
        // game loop
        foreach (var @event in ManagedEvents.OfType<DayEndingEvent>())
            modEvents.GameLoop.DayEnding += @event.OnDayEnding;

        foreach (var @event in ManagedEvents.OfType<DayStartedEvent>())
            modEvents.GameLoop.DayStarted += @event.OnDayStarted;

        foreach (var @event in ManagedEvents.OfType<GameLaunchedEvent>())
            modEvents.GameLoop.GameLaunched += @event.OnGameLaunched;

        foreach (var @event in ManagedEvents.OfType<OneSecondUpdateTickedEvent>())
            modEvents.GameLoop.OneSecondUpdateTicked += @event.OnOneSecondUpdateTicked;

        foreach (var @event in ManagedEvents.OfType<OneSecondUpdateTickingEvent>())
            modEvents.GameLoop.OneSecondUpdateTicking += @event.OnOneSecondUpdateTicking;

        foreach (var @event in ManagedEvents.OfType<ReturnedToTitleEvent>())
            modEvents.GameLoop.ReturnedToTitle += @event.OnReturnedToTitle;

        foreach (var @event in ManagedEvents.OfType<SaveCreatedEvent>())
            modEvents.GameLoop.SaveCreated += @event.OnSaveCreated;

        foreach (var @event in ManagedEvents.OfType<SaveCreatingEvent>())
            modEvents.GameLoop.SaveCreating += @event.OnSaveCreating;

        foreach (var @event in ManagedEvents.OfType<SaveLoadedEvent>())
            modEvents.GameLoop.SaveLoaded += @event.OnSaveLoaded;

        foreach (var @event in ManagedEvents.OfType<SavingEvent>())
            modEvents.GameLoop.Saving += @event.OnSaving;

        foreach (var @event in ManagedEvents.OfType<TimeChangedEvent>())
            modEvents.GameLoop.TimeChanged += @event.OnTimeChanged;

        foreach (var @event in ManagedEvents.OfType<UpdateTickedEvent>())
            modEvents.GameLoop.UpdateTicked += @event.OnUpdateTicked;

        foreach (var @event in ManagedEvents.OfType<UpdateTickingEvent>())
            modEvents.GameLoop.UpdateTicking += @event.OnUpdateTicking;

        // input
        foreach (var @event in ManagedEvents.OfType<ButtonPressedEvent>())
            modEvents.Input.ButtonPressed += @event.OnButtonPressed;

        foreach (var @event in ManagedEvents.OfType<ButtonReleasedEvent>())
            modEvents.Input.ButtonReleased += @event.OnButtonReleased;

        foreach (var @event in ManagedEvents.OfType<ButtonsChangedEvent>())
            modEvents.Input.ButtonsChanged += @event.OnButtonsChanged;

        foreach (var @event in ManagedEvents.OfType<CursorMovedEvent>())
            modEvents.Input.CursorMoved += @event.OnCursorMoved;

        foreach (var @event in ManagedEvents.OfType<MouseWheelScrolledEvent>())
            modEvents.Input.MouseWheelScrolled += @event.OnMouseWheelScrolled;

        // multiplayer
        foreach (var @event in ManagedEvents.OfType<ModMessageReceivedEvent>())
            modEvents.Multiplayer.ModMessageReceived += @event.OnModMessageReceived;

        foreach (var @event in ManagedEvents.OfType<PeerConnectedEvent>())
            modEvents.Multiplayer.PeerConnected += @event.OnPeerConnected;

        foreach (var @event in ManagedEvents.OfType<PeerContextReceivedEvent>())
            modEvents.Multiplayer.PeerContextReceived += @event.OnPeerContextReceived;

        foreach (var @event in ManagedEvents.OfType<PeerDisconnectedEvent>())
            modEvents.Multiplayer.PeerDisconnected += @event.OnPeerDisconnected;

        // player
        foreach (var @event in ManagedEvents.OfType<InventoryChangedEvent>())
            modEvents.Player.InventoryChanged += @event.OnInventoryChanged;

        foreach (var @event in ManagedEvents.OfType<LevelChangedEvent>())
            modEvents.Player.LevelChanged += @event.OnLevelChanged;

        foreach (var @event in ManagedEvents.OfType<WarpedEvent>())
            modEvents.Player.Warped += @event.OnWarped;

        // world
        foreach (var @event in ManagedEvents.OfType<BuildingListChangedEvent>())
            modEvents.World.BuildingListChanged += @event.OnBuildingListChanged;

        foreach (var @event in ManagedEvents.OfType<ChestInventoryChangedEvent>())
            modEvents.World.ChestInventoryChanged += @event.OnChestInventoryChanged;

        foreach (var @event in ManagedEvents.OfType<DebrisListChangedEvent>())
            modEvents.World.DebrisListChanged += @event.OnDebrisListChanged;

        foreach (var @event in ManagedEvents.OfType<FurnitureListChangedEvent>())
            modEvents.World.FurnitureListChanged += @event.OnFurnitureListChanged;

        foreach (var @event in ManagedEvents.OfType<LargeTerrainFeatureListChangedEvent>())
            modEvents.World.LargeTerrainFeatureListChanged += @event.OnLargeTerrainFeatureListChanged;

        foreach (var @event in ManagedEvents.OfType<LocationListChangedEvent>())
            modEvents.World.LocationListChanged += @event.OnLocationListChanged;

        foreach (var @event in ManagedEvents.OfType<NpcListChangedEvent>())
            modEvents.World.NpcListChanged += @event.OnNpcListChanged;

        foreach (var @event in ManagedEvents.OfType<ObjectListChangedEvent>())
            modEvents.World.ObjectListChanged += @event.OnObjectListChanged;

        foreach (var @event in ManagedEvents.OfType<TerrainFeatureListChangedEvent>())
            modEvents.World.TerrainFeatureListChanged += @event.OnTerrainFeatureListChanged;

        // specialized
        foreach (var @event in ManagedEvents.OfType<LoadStageChangedEvent>())
            modEvents.Specialized.LoadStageChanged += @event.OnLoadStageChanged;

        foreach (var @event in ManagedEvents.OfType<UnvalidatedUpdateTickedEvent>())
            modEvents.Specialized.UnvalidatedUpdateTicked += @event.OnUnvalidatedUpdateTicked;

        foreach (var @event in ManagedEvents.OfType<UnvalidatedUpdateTickingEvent>())
            modEvents.Specialized.UnvalidatedUpdateTicking += @event.OnUnvalidatedUpdateTicking;

        #endregion hookers

        Log.D("[EventManager]: Initialization of SMAPI events completed.");
    }

    /// <summary>Enumerate all managed event instances.</summary>
    internal IEnumerable<IManagedEvent> Events => ManagedEvents;

    /// <summary>Enumerate all currently hooked events for the local player.</summary>
    internal IEnumerable<IManagedEvent> Hooked => ManagedEvents.Where(e => e.IsHooked);

    /// <summary>Hook a single <see cref="IManagedEvent"/>.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to hook.</typeparam>
    internal void Hook<TEvent>() where TEvent : IManagedEvent
    {
        var e = Get<TEvent>();
        if (e is null)
        {
            Log.D($"[EventManager]: The type {typeof(TEvent).Name} was not found.");
            return;
        }

        e.Hook();
        Log.D($"[EventManager]: Hooked {typeof(TEvent).Name}.");
    }

    /// <summary>Hook the specified <see cref="IManagedEvent"/> types.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to hook.</param>
    internal void Hook(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IManagedEvent)) || type.IsAbstract)
            {
                Log.D($"[EventManager]: {type.Name} is not a valid event type.");
                continue;
            }

            var e = ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.D($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            e.Hook();
            Log.D($"[EventManager]: Hooked {type.Name}.");
        }
    }

    /// <summary>Unhook a single <see cref="IManagedEvent"/>.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to unhook.</typeparam>
    internal void Unhook<TEvent>() where TEvent : IManagedEvent
    {
        var e = Get<TEvent>();
        if (e is null)
        {
            Log.D($"[EventManager]: The type {typeof(TEvent).Name} was not found.");
            return;
        }

        e.Unhook();
        Log.D($"[EventManager]: Unhook {typeof(TEvent).Name}.");
    }

    /// <summary>Unhook events from the event listener.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to unhook.</param>
    internal void Unhook(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IManagedEvent)) || type.IsAbstract)
            {
                Log.D($"[EventManager]: {type.Name} is not a valid event type.");
                continue;
            }

            var e = ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.D($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            e.Unhook();
            Log.D($"[EventManager]: Unhooked {type.Name}.");
        }
    }

    /// <summary>Hook all <see cref="IManagedEvent"/>s in the assembly.</summary>
    /// <param name="except">Types to be excluded, if any.</param>
    internal void HookAll(params Type[] except)
    {
        Log.D($"[EventManager]: Hooking all events...");
        var toHook = ManagedEvents
            .Select(e => e.GetType())
            .Except(except)
            .ToArray();
        Hook(toHook);
        Log.D($"Hooked {toHook.Length} events.");
    }

    /// <summary>Unhook all <see cref="IManagedEvent"/>s in the assembly.</summary>
    /// <param name="except">Types to be excluded, if any.</param>
    internal void UnhookAll(params Type[] except)
    {
        Log.D($"[EventManager]: Unhooking all events...");
        var toUnhook = ManagedEvents
            .Select(e => e.GetType())
            .Except(except)
            .ToArray();
        Hook(toUnhook);
        Log.D($"Unhooked {toUnhook.Length} events.");
    }

    /// <summary>Unhook all <see cref="IManagedEvent"/> types starting with the specified prefix.</summary>
    /// <param name="prefix">A <see cref="string"/> prefix.</param>
    /// <param name="except">Types to be excluded, if any.</param>
    internal void HookStartingWith(string prefix, params Type[] except)
    {
        Log.D($"[EventManager]: Searching for '{prefix}' events...");
        var toHook = ManagedEvents
            .Select(e => e.GetType())
            .Where(t => t.Name.StartsWith(prefix))
            .Except(except)
            .ToArray();
        Hook(toHook);
        Log.D($"Hooked {toHook.Length} events.");
    }

    /// <summary>Unhook all <see cref="IManagedEvent"/> types starting with the specified prefix.</summary>
    /// <param name="prefix">A <see cref="string" /> prefix.</param>
    /// <param name="except">Types to be excluded, if any.</param>
    internal void UnhookStartingWith(string prefix, params Type[] except)
    {
        Log.D($"[EventManager]: Searching for events beginning with '{prefix}'...");
        var toUnhook = ManagedEvents
            .Select(e => e.GetType())
            .Where(t => t.Name.StartsWith(prefix))
            .Except(except)
            .ToArray();
        Unhook(toUnhook);
        Log.D($"[EventManager]: Unhooked {toUnhook.Length} events.");
    }

    /// <summary>Hook save-dependent events.</summary>
    internal virtual void HookForLocalPlayer()
    {
    }

    /// <summary>Unhook save-dependent events.</summary>
    internal virtual void UnhookFromLocalPlayer()
    {
        Log.D("[EventManager]: Unhooking local player events...");
        var toUnhook = ManagedEvents
            .Select(e => e.GetType())
            .Where(t => !t.IsAssignableToAnyOf(typeof(GameLaunchedEvent), typeof(SaveCreatedEvent),
                    typeof(SaveCreatingEvent), typeof(SaveLoadedEvent), typeof(ReturnedToTitleEvent)))
            .ToArray();
        Unhook(toUnhook);
        Log.D($"[EventManager]: Unhooked {toUnhook.Length} events.");
    }

    /// <summary>Add a new event instance to the set of managed events.</summary>
    /// <param name="event">An <see cref="IManagedEvent"/> instance.</param>
    internal bool Manage(IManagedEvent @event)
    {
        return ManagedEvents.Add(@event);
    }

    /// <summary>Add a new event instance to the set of managed events.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</param>
    internal void Manage<TEvent>() where TEvent : IManagedEvent, new()
    {
        if (!TryGet<TEvent>(out _)) ManagedEvents.Add(new TEvent());
    }

    /// <summary>Get an instance of the specified event type.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    internal TEvent? Get<TEvent>() where TEvent : IManagedEvent
    {
        return ManagedEvents.OfType<TEvent>().FirstOrDefault();
    }

    /// <summary>Try to get an instance of the specified event type.</summary>
    /// <param name="got">The matched event, if any.</param>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <returns><see langword="true"> if a matching event was found, otherwise <see langword="false">.</returns>
    internal bool TryGet<TEvent>([NotNullWhen(true)] out TEvent? got) where TEvent : IManagedEvent
    {
        got = Get<TEvent>();
        return got is not null;
    }

    /// <summary>Check if the specified event type is hooked.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    internal bool IsHooked<TEvent>() where TEvent : IManagedEvent
    {
        return TryGet<TEvent>(out var got) && got?.IsHooked == true;
    }

    /// <summary>Enumerate all currently hooked event for the specified screen.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    internal IEnumerable<IManagedEvent> GetHookedForScreen(int screenID)
    {
        return ManagedEvents.Where(e => e.IsHookedForScreen(screenID));
    }
}