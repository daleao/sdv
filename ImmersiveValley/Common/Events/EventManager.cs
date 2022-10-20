namespace DaLion.Common.Events;

#region using directives

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using DaLion.Common.Attributes;
using DaLion.Common.Commands;
using DaLion.Common.Extensions.Collections;
using HarmonyLib;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>
///     Instantiates and manages dynamic enabling and disabling of <see cref="IManagedEvent"/> classes in the
///     assembly.
/// </summary>
internal class EventManager
{
    /// <summary>Initializes a new instance of the <see cref="EventManager"/> class.</summary>
    /// <param name="modEvents">The <see cref="IModEvents"/> API for the current mod.</param>
    internal EventManager(IModEvents modEvents)
    {
        this.ModEvents = modEvents;

        Log.D("[EventManager]: Gathering events...");
        var eventTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract &&
                        // event classes may or not have the required internal parameterized constructor accepting only the manager instance, depending on whether they are SMAPI or mod-handled
                        // we only want to construct SMAPI events at this point, so we filter out the rest
                        t.GetConstructor(
                            BindingFlags.Instance | BindingFlags.NonPublic,
                            null,
                            new[] { this.GetType() },
                            null) is not null)
            .ToArray();

        Log.D($"[EventManager]: Found {eventTypes.Length} event classes. Initializing events...");
        foreach (var e in eventTypes)
        {
            try
            {
#if RELEASE
                var debugOnlyAttribute =
                    (DebugOnlyAttribute?)e.GetCustomAttributes(typeof(DebugOnlyAttribute), false).FirstOrDefault();
                if (debugOnlyAttribute is not null) continue;
#endif

                var deprecatedAttr =
                    (DeprecatedAttribute?)e.GetCustomAttributes(typeof(DeprecatedAttribute), false).FirstOrDefault();
                if (deprecatedAttr is not null)
                {
                    continue;
                }

                var @event = (IManagedEvent)e
                    .GetConstructor(
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        null,
                        new[] { this.GetType() },
                        null)!
                    .Invoke(new object?[] { this });
                this.ManagedEvents.Add(@event);
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
        foreach (var @event in this.ManagedEvents.OfType<AssetReadyEvent>())
        {
            modEvents.Content.AssetReady += @event.OnAssetReady;
        }

        foreach (var @event in this.ManagedEvents.OfType<AssetRequestedEvent>())
        {
            modEvents.Content.AssetRequested += @event.OnAssetRequested;
        }

        foreach (var @event in this.ManagedEvents.OfType<AssetsInvalidatedEvent>())
        {
            modEvents.Content.AssetsInvalidated += @event.OnAssetsInvalidated;
        }

        foreach (var @event in this.ManagedEvents.OfType<LocaleChangedEvent>())
        {
            modEvents.Content.LocaleChanged += @event.OnLocaleChanged;
        }

        // display
        foreach (var @event in this.ManagedEvents.OfType<MenuChangedEvent>())
        {
            modEvents.Display.MenuChanged += @event.OnMenuChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<RenderedActiveMenuEvent>())
        {
            modEvents.Display.RenderedActiveMenu += @event.OnRenderedActiveMenu;
        }

        foreach (var @event in this.ManagedEvents.OfType<RenderedHudEvent>())
        {
            modEvents.Display.RenderedHud += @event.OnRenderedHud;
        }

        foreach (var @event in this.ManagedEvents.OfType<RenderedWorldEvent>())
        {
            modEvents.Display.RenderedWorld += @event.OnRenderedWorld;
        }

        foreach (var @event in this.ManagedEvents.OfType<RenderingEvent>())
        {
            modEvents.Display.Rendering += @event.OnRendering;
        }

        foreach (var @event in this.ManagedEvents.OfType<RenderingHudEvent>())
        {
            modEvents.Display.RenderingHud += @event.OnRenderingHud;
        }

        foreach (var @event in this.ManagedEvents.OfType<RenderingWorldEvent>())
        {
            modEvents.Display.RenderingWorld += @event.OnRenderingWorld;
        }

        foreach (var @event in this.ManagedEvents.OfType<WindowResizedEvent>())
        {
            modEvents.Display.WindowResized += @event.OnWindowResized;
        }

        // game loop
        foreach (var @event in this.ManagedEvents.OfType<DayEndingEvent>())
        {
            modEvents.GameLoop.DayEnding += @event.OnDayEnding;
        }

        foreach (var @event in this.ManagedEvents.OfType<DayStartedEvent>())
        {
            modEvents.GameLoop.DayStarted += @event.OnDayStarted;
        }

        foreach (var @event in this.ManagedEvents.OfType<GameLaunchedEvent>())
        {
            modEvents.GameLoop.GameLaunched += @event.OnGameLaunched;
        }

        foreach (var @event in this.ManagedEvents.OfType<OneSecondUpdateTickedEvent>())
        {
            modEvents.GameLoop.OneSecondUpdateTicked += @event.OnOneSecondUpdateTicked;
        }

        foreach (var @event in this.ManagedEvents.OfType<OneSecondUpdateTickingEvent>())
        {
            modEvents.GameLoop.OneSecondUpdateTicking += @event.OnOneSecondUpdateTicking;
        }

        foreach (var @event in this.ManagedEvents.OfType<ReturnedToTitleEvent>())
        {
            modEvents.GameLoop.ReturnedToTitle += @event.OnReturnedToTitle;
        }

        foreach (var @event in this.ManagedEvents.OfType<SaveCreatedEvent>())
        {
            modEvents.GameLoop.SaveCreated += @event.OnSaveCreated;
        }

        foreach (var @event in this.ManagedEvents.OfType<SaveCreatingEvent>())
        {
            modEvents.GameLoop.SaveCreating += @event.OnSaveCreating;
        }

        foreach (var @event in this.ManagedEvents.OfType<SavedEvent>())
        {
            modEvents.GameLoop.Saved += @event.OnSaved;
        }

        foreach (var @event in this.ManagedEvents.OfType<SaveLoadedEvent>())
        {
            modEvents.GameLoop.SaveLoaded += @event.OnSaveLoaded;
        }

        foreach (var @event in this.ManagedEvents.OfType<SavingEvent>())
        {
            modEvents.GameLoop.Saving += @event.OnSaving;
        }

        foreach (var @event in this.ManagedEvents.OfType<TimeChangedEvent>())
        {
            modEvents.GameLoop.TimeChanged += @event.OnTimeChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<UpdateTickedEvent>())
        {
            modEvents.GameLoop.UpdateTicked += @event.OnUpdateTicked;
        }

        foreach (var @event in this.ManagedEvents.OfType<UpdateTickingEvent>())
        {
            modEvents.GameLoop.UpdateTicking += @event.OnUpdateTicking;
        }

        // input
        foreach (var @event in this.ManagedEvents.OfType<ButtonPressedEvent>())
        {
            modEvents.Input.ButtonPressed += @event.OnButtonPressed;
        }

        foreach (var @event in this.ManagedEvents.OfType<ButtonReleasedEvent>())
        {
            modEvents.Input.ButtonReleased += @event.OnButtonReleased;
        }

        foreach (var @event in this.ManagedEvents.OfType<ButtonsChangedEvent>())
        {
            modEvents.Input.ButtonsChanged += @event.OnButtonsChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<CursorMovedEvent>())
        {
            modEvents.Input.CursorMoved += @event.OnCursorMoved;
        }

        foreach (var @event in this.ManagedEvents.OfType<MouseWheelScrolledEvent>())
        {
            modEvents.Input.MouseWheelScrolled += @event.OnMouseWheelScrolled;
        }

        // multiplayer
        foreach (var @event in this.ManagedEvents.OfType<ModMessageReceivedEvent>())
        {
            modEvents.Multiplayer.ModMessageReceived += @event.OnModMessageReceived;
        }

        foreach (var @event in this.ManagedEvents.OfType<PeerConnectedEvent>())
        {
            modEvents.Multiplayer.PeerConnected += @event.OnPeerConnected;
        }

        foreach (var @event in this.ManagedEvents.OfType<PeerContextReceivedEvent>())
        {
            modEvents.Multiplayer.PeerContextReceived += @event.OnPeerContextReceived;
        }

        foreach (var @event in this.ManagedEvents.OfType<PeerDisconnectedEvent>())
        {
            modEvents.Multiplayer.PeerDisconnected += @event.OnPeerDisconnected;
        }

        // player
        foreach (var @event in this.ManagedEvents.OfType<InventoryChangedEvent>())
        {
            modEvents.Player.InventoryChanged += @event.OnInventoryChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<LevelChangedEvent>())
        {
            modEvents.Player.LevelChanged += @event.OnLevelChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<WarpedEvent>())
        {
            modEvents.Player.Warped += @event.OnWarped;
        }

        // world
        foreach (var @event in this.ManagedEvents.OfType<BuildingListChangedEvent>())
        {
            modEvents.World.BuildingListChanged += @event.OnBuildingListChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<ChestInventoryChangedEvent>())
        {
            modEvents.World.ChestInventoryChanged += @event.OnChestInventoryChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<DebrisListChangedEvent>())
        {
            modEvents.World.DebrisListChanged += @event.OnDebrisListChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<FurnitureListChangedEvent>())
        {
            modEvents.World.FurnitureListChanged += @event.OnFurnitureListChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<LargeTerrainFeatureListChangedEvent>())
        {
            modEvents.World.LargeTerrainFeatureListChanged += @event.OnLargeTerrainFeatureListChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<LocationListChangedEvent>())
        {
            modEvents.World.LocationListChanged += @event.OnLocationListChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<NpcListChangedEvent>())
        {
            modEvents.World.NpcListChanged += @event.OnNpcListChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<ObjectListChangedEvent>())
        {
            modEvents.World.ObjectListChanged += @event.OnObjectListChanged;
        }

        foreach (var @event in this.ManagedEvents.OfType<TerrainFeatureListChangedEvent>())
        {
            modEvents.World.TerrainFeatureListChanged += @event.OnTerrainFeatureListChanged;
        }

        //// specialized
        //foreach (var @event in ManagedEvents.OfType<LoadStageChangedEvent>())
        //    modEvents.Specialized.LoadStageChanged += @event.OnLoadStageChanged;

        //foreach (var @event in ManagedEvents.OfType<UnvalidatedUpdateTickedEvent>())
        //    modEvents.Specialized.UnvalidatedUpdateTicked += @event.OnUnvalidatedUpdateTicked;

        //foreach (var @event in ManagedEvents.OfType<UnvalidatedUpdateTickingEvent>())
        //    modEvents.Specialized.UnvalidatedUpdateTicking += @event.OnUnvalidatedUpdateTicking;

        #endregion hookers

        Log.D("[EventManager]: Initialization of SMAPI events completed.");

        PrintEnabledEventsCommand.Manager = this;
    }

    /// <summary>Gets an enumerable of all <see cref="IManagedEvent"/>s instances.</summary>
    internal IEnumerable<IManagedEvent> Managed => this.ManagedEvents;

    /// <summary>Gets an enumerable of all <see cref="IManagedEvent"/>s currently enabled for the local player.</summary>
    internal IEnumerable<IManagedEvent> Enabled => this.ManagedEvents.Where(e => e.IsEnabled);

    /// <summary>Gets the cached <see cref="IManagedEvent"/> instances.</summary>
    protected HashSet<IManagedEvent> ManagedEvents { get; } = new();

    /// <inheritdoc cref="IModEvents"/>
    protected IModEvents ModEvents { get; }

    /// <summary>Enumerates all <see cref="IManagedEvent"/>s currently enabled for the specified screen.</summary>
    /// <param name="screenId">The screen ID.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of enabled <see cref="IManagedEvent"/>s in the specified screen.</returns>
    internal IEnumerable<IManagedEvent> EnabledForScreen(int screenId)
    {
        return this.ManagedEvents.Where(e => e.IsEnabledForScreen(screenId));
    }

    /// <summary>Enable a single <see cref="IManagedEvent"/>.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to enable.</typeparam>
    internal void Enable<TEvent>()
        where TEvent : IManagedEvent
    {
        var e = this.Get<TEvent>();
        if (e is null)
        {
            Log.D($"[EventManager]: The type {typeof(TEvent).Name} was not found.");
            return;
        }

        if (e.Enable())
        {
            Log.D($"[EventManager]: Enabled {typeof(TEvent).Name}.");
        }
    }

    /// <summary>Enables the specified <see cref="IManagedEvent"/> types.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to enable.</param>
    internal void Enable(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IManagedEvent)) || type.IsAbstract)
            {
                Log.D($"[EventManager]: {type.Name} is not a valid event type.");
                continue;
            }

            var e = this.ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.D($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            if (e.Enable())
            {
                Log.D($"[EventManager]: Enabled {type.Name}.");
            }
        }
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to enable.</typeparam>
    /// <param name="screenId">A local peer's screen ID.</param>
    internal void EnableForScreen<TEvent>(int screenId)
        where TEvent : IManagedEvent
    {
        var e = this.Get<TEvent>();
        if (e is null)
        {
            Log.D($"[EventManager]: The type {typeof(TEvent).Name} was not found.");
            return;
        }

        if (e.EnableForScreen(screenId))
        {
            Log.D($"[EventManager]: Enabled {typeof(TEvent).Name} for screen {screenId}.");
        }
    }

    /// <summary>Enables the specified <see cref="IManagedEvent"/> types for the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to enable.</param>
    internal void EnableForScreen(int screenId, params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IManagedEvent)) || type.IsAbstract)
            {
                Log.D($"[EventManager]: {type.Name} is not a valid event type.");
                continue;
            }

            var e = this.ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.D($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            if (e.EnableForScreen(screenId))
            {
                Log.D($"[EventManager]: Enabled {type.Name} for screen {screenId}.");
            }
        }
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to enable.</typeparam>
    internal void EnableForAllScreens<TEvent>()
        where TEvent : IManagedEvent
    {
        var e = this.Get<TEvent>();
        if (e is null)
        {
            Log.D($"[EventManager]: The type {typeof(TEvent).Name} was not found.");
            return;
        }

        e.EnableForAllScreens();
        Log.D($"[EventManager]: Enabled {typeof(TEvent).Name} for all screens.");
    }

    /// <summary>Enables the specified <see cref="IManagedEvent"/> types for the specified screen.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to enable.</param>
    internal void EnableForAllScreens(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IManagedEvent)) || type.IsAbstract)
            {
                Log.D($"[EventManager]: {type.Name} is not a valid event type.");
                continue;
            }

            var e = this.ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.D($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            e.EnableForAllScreens();
            Log.D($"[EventManager]: Enabled {type.Name} for all screens.");
        }
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/>.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to disable.</typeparam>
    internal void Disable<TEvent>()
        where TEvent : IManagedEvent
    {
        var e = this.Get<TEvent>();
        if (e is null)
        {
            Log.D($"[EventManager]: The type {typeof(TEvent).Name} was not found.");
            return;
        }

        if (e.Disable())
        {
            Log.D($"[EventManager]: Disabled {typeof(TEvent).Name}.");
        }
    }

    /// <summary>Disables the specified <see cref="IManagedEvent"/>s events.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to disable.</param>
    internal void Disable(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IManagedEvent)) || type.IsAbstract)
            {
                Log.D($"[EventManager]: {type.Name} is not a valid event type.");
                continue;
            }

            var e = this.ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.D($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            if (e.Disable())
            {
                Log.D($"[EventManager]: Disabled {type.Name}.");
            }
        }
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to disable.</typeparam>
    /// <param name="screenId">A local peer's screen ID.</param>
    internal void DisableForScreen<TEvent>(int screenId)
        where TEvent : IManagedEvent
    {
        var e = this.Get<TEvent>();
        if (e is null)
        {
            Log.D($"[EventManager]: The type {typeof(TEvent).Name} was not found.");
            return;
        }

        if (e.DisableForScreen(screenId))
        {
            Log.D($"[EventManager]: Disabled {typeof(TEvent).Name} for screen {screenId}.");
        }
    }

    /// <summary>Disables the specified <see cref="IManagedEvent"/>s for the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to disable.</param>
    internal void DisableForScreen(int screenId, params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IManagedEvent)) || type.IsAbstract)
            {
                Log.D($"[EventManager]: {type.Name} is not a valid event type.");
                continue;
            }

            var e = this.ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.D($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            if (e.DisableForScreen(screenId))
            {
                Log.D($"[EventManager]: Disabled {type.Name} for screen {screenId}.");
            }
        }
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to disable.</typeparam>
    internal void DisableForAllScreens<TEvent>()
        where TEvent : IManagedEvent
    {
        var e = this.Get<TEvent>();
        if (e is null)
        {
            Log.D($"[EventManager]: The type {typeof(TEvent).Name} was not found.");
            return;
        }

        e.DisableForAllScreens();
        Log.D($"[EventManager]: Disabled {typeof(TEvent).Name} for all screens.");
    }

    /// <summary>Disables the specified <see cref="IManagedEvent"/>s for the specified screen.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to disable.</param>
    internal void DisableForAllScreens(params Type[] eventTypes)
    {
        foreach (var type in eventTypes)
        {
            if (!type.IsAssignableTo(typeof(IManagedEvent)) || type.IsAbstract)
            {
                Log.D($"[EventManager]: {type.Name} is not a valid event type.");
                continue;
            }

            var e = this.ManagedEvents.FirstOrDefault(e => e.GetType() == type);
            if (e is null)
            {
                Log.D($"[EventManager]: The type {type.Name} was not found.");
                continue;
            }

            e.DisableForAllScreens();
            Log.D($"[EventManager]: Disabled {type.Name} for all screens.");
        }
    }

    /// <summary>Enables all <see cref="IManagedEvent"/>s in the assembly.</summary>
    internal void EnableAll()
    {
        Log.D("[EventManager]: Enabling all events...");
        var toEnable = this.ManagedEvents
            .Select(e => e.GetType())
            .ToArray();
        this.Enable(toEnable);
        Log.D($"Enabled {toEnable.Length} events.");
    }

    /// <summary>Disables all <see cref="IManagedEvent"/>s in the assembly.</summary>
    internal void DisableAll()
    {
        Log.D("[EventManager]: Disabling all events...");
        var toDisable = this.ManagedEvents
            .Select(e => e.GetType())
            .ToArray();
        this.Disable(toDisable);
        Log.D($"Disabled {toDisable.Length} events.");
    }

    /// <summary>Enables all <see cref="IManagedEvent"/> types starting with attribute <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    internal void EnableWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        Log.D($"[EventManager]: Searching for events with {typeof(TAttribute).Name}...");
        var toEnable = this.ManagedEvents
            .Select(e => e.GetType())
            .Where(t => t.GetCustomAttribute<TAttribute>() is not null)
            .ToArray();
        this.Enable(toEnable);
        Log.D($"[EventManager]: Enabled {toEnable.Length} events.");
    }

    /// <summary>Disables all <see cref="IManagedEvent"/> types starting with attribute <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    internal void DisableWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        Log.D($"[EventManager]: Searching for events beginning with {typeof(TAttribute).Name}...");
        var toDisable = this.ManagedEvents
            .Select(e => e.GetType())
            .Where(t => t.GetCustomAttribute<TAttribute>() is not null)
            .ToArray();
        this.Disable(toDisable);
        Log.D($"[EventManager]: Disabled {toDisable.Length} events.");
    }

    /// <summary>Resets the enabled status of all <see cref="IManagedEvent"/>s in the assembly for all screens.</summary>
    internal void ResetAllScreens()
    {
        this.ManagedEvents.ForEach(e => e.Reset());
        Log.D("[EventManager]: All managed events were reset.");
    }

    /// <summary>Adds a new <see cref="IManagedEvent"/> instance to the set of <see cref="ManagedEvents"/>.</summary>
    /// <param name="event">An <see cref="IManagedEvent"/> instance.</param>
    /// <returns><see langword="true"/> if the event wasn't already managed, otherwise <see langword="false"/>.</returns>
    internal bool Manage(IManagedEvent @event)
    {
        return this.ManagedEvents.Add(@event);
    }

    /// <summary>Adds a new <see cref="IManagedEvent"/> instance to the set of <see cref="ManagedEvents"/>.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    internal void Manage<TEvent>()
        where TEvent : IManagedEvent, new()
    {
        if (!this.TryGet<TEvent>(out _))
        {
            this.ManagedEvents.Add(new TEvent());
        }
    }

    /// <summary>Gets the instance of the specified <see cref="IManagedEvent"/> type.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <returns>The instance of type <typeparamref name="TEvent"/>.</returns>
    internal TEvent? Get<TEvent>()
        where TEvent : IManagedEvent
    {
        return this.ManagedEvents.OfType<TEvent>().FirstOrDefault();
    }

    /// <summary>
    ///     Attempts to get an instance of the specified <see cref="IManagedEvent"/> type, and returns whether the parse
    ///     was successful.
    /// </summary>
    /// <param name="got">The matched event, if any.</param>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <returns><see langword="true"/> if a matching event was found, otherwise <see langword="false"/>.</returns>
    internal bool TryGet<TEvent>([NotNullWhen(true)] out TEvent? got)
        where TEvent : IManagedEvent
    {
        got = this.Get<TEvent>();
        return got is not null;
    }

    /// <summary>Determines whether the specified <see cref="IManagedEvent"/> type is enabled.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <returns><see langword="true"/> if the <see cref="IManagedEvent"/> is enabled for the local screen, otherwise <see langword="false"/>.</returns>
    internal bool IsEnabled<TEvent>()
        where TEvent : IManagedEvent
    {
        return this.TryGet<TEvent>(out var got) && got.IsEnabled;
    }

    /// <summary>Determines whether the specified <see cref="IManagedEvent"/> type is enabled for a specific screen.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <param name="screenId">The screen ID.</param>
    /// <returns><see langword="true"/> if the <see cref="IManagedEvent"/> is enabled for the specified screen, otherwise <see langword="false"/>.</returns>
    internal bool IsEnabledForScreen<TEvent>(int screenId)
        where TEvent : IManagedEvent
    {
        return this.TryGet<TEvent>(out var got) && got.IsEnabledForScreen(screenId);
    }
}
