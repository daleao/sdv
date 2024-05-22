namespace DaLion.Shared.Events;

#region using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>
///     Instantiates and manages dynamic enabling and disabling of <see cref="IManagedEvent"/> classes in an
///     assembly or namespace.
/// </summary>
public sealed class EventManager
{
    /// <summary>Cache of <see cref="IManagedEvent"/> instances by type.</summary>
    private readonly ConditionalWeakTable<Type, IManagedEvent> _eventCache = [];

    /// <inheritdoc cref="IModRegistry"/>
    private readonly IModRegistry _modRegistry;

    /// <summary>Initializes a new instance of the <see cref="EventManager"/> class.</summary>
    /// <param name="modEvents">The <see cref="IModEvents"/> API for the current mod.</param>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    public EventManager(IModEvents modEvents, IModRegistry modRegistry, Logger logger)
    {
        this._modRegistry = modRegistry;
        this.Log = logger;
        this.ModEvents = modEvents;
    }

    /// <inheritdoc cref="Logger"/>
    public Logger Log { get; }

    /// <inheritdoc cref="IModEvents"/>
    public IModEvents ModEvents { get; }

    /// <summary>Gets an enumerable of all <see cref="IManagedEvent"/>s instances.</summary>
    public IEnumerable<IManagedEvent> Managed => this._eventCache.Select(pair => pair.Value);

    /// <summary>Gets an enumerable of all <see cref="IManagedEvent"/>s currently enabled for the local player.</summary>
    public IEnumerable<IManagedEvent> Enabled => this.Managed.Where(e => e.IsEnabled);

    /// <summary>Enumerates all <see cref="IManagedEvent"/>s currently enabled for the specified screen.</summary>
    /// <param name="screenId">The screen ID.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of enabled <see cref="IManagedEvent"/>s in the specified screen.</returns>
    public IEnumerable<IManagedEvent> EnabledForScreen(int screenId)
    {
        return this.Managed
            .Where(e => e.IsEnabledForScreen(screenId));
    }

    /// <summary>Adds the <paramref name="event"/> instance to the cache.</summary>
    /// <param name="event">An <see cref="IManagedEvent"/> instance.</param>
    public void Manage(IManagedEvent @event)
    {
        this._eventCache.Add(@event.GetType(), @event);
        this.Log.D($"[EventManager]: Now managing {@event.GetType().Name}.");
    }

    /// <summary>Adds an instance of type <typeparamref name="TEvent"/> to the cache.</summary>
    /// <typeparam name="TEvent">A <see cref="IManagedEvent"/> type to disable.</typeparam>
    /// <param name="parameters">The parameters for the constructor of <typeparamref name="TEvent"/>.</param>
    public void Manage<TEvent>(params object?[]? parameters)
        where TEvent : IManagedEvent
    {
        if (parameters is not null)
        {
            parameters = this.Collect(parameters).ToArray();
        }

        var instance = typeof(TEvent).RequireConstructor(parameters?.Length ?? 1).Invoke(parameters ?? [this]);
        if (instance is not TEvent @event)
        {
            ThrowHelper.ThrowInvalidOperationException($"Failed to construct instance of type {typeof(TEvent).Name}");
            return;
        }

        this._eventCache.AddOrUpdate(typeof(TEvent), @event);
        this.Log.D($"[EventManager]: Now managing {typeof(TEvent).Name}.");
    }

    /// <summary>Implicitly manages all <see cref="IManagedEvent"/> types in the specified <paramref name="assembly"/>> using reflection.</summary>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <returns>The <see cref="EventManager"/> instance.</returns>
    public EventManager ManageInitial(Assembly assembly)
    {
        this.Log.D("[EventManager]: Gathering all events...");
        this.ManageImplicitly(assembly, t =>
            t.IsAssignableToAnyOf(typeof(GameLaunchedEvent), typeof(FirstSecondUpdateTickedEvent)) ||
            t.HasAttribute<AlwaysEnabledEventAttribute>() ||
            t.GetProperty(nameof(IManagedEvent.IsEnabled))!.DeclaringType == t);
        return this;
    }

    /// <summary>Implicitly manages all <see cref="IManagedEvent"/> types in the specified <paramref name="assembly"/>> using reflection.</summary>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <returns>The <see cref="EventManager"/> instance.</returns>
    public EventManager ManageAll(Assembly assembly)
    {
        this.Log.D("[EventManager]: Gathering all events...");
        this.ManageImplicitly(assembly);
        return this;
    }

    /// <summary>Implicitly manages only the <see cref="IManagedEvent"/> types in the specified <paramref name="assembly"/>> which are also within the specified <paramref name="namespace"/>.</summary>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <param name="namespace">The desired namespace.</param>
    /// <returns>The <see cref="EventManager"/> instance.</returns>
    public EventManager ManageNamespace(Assembly assembly, string @namespace)
    {
        this.Log.D($"[EventManager]: Gathering events in {@namespace}...");
        this.ManageImplicitly(assembly, t => t.Namespace?.Contains(@namespace) == true);
        return this;
    }

    /// <summary>Implicitly manages only the <see cref="IManagedEvent"/> types in the specified <paramref name="assembly"/>> which are also decorated with <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    /// <param name="assembly">The assembly containing the types.</param>
    /// <returns>The <see cref="EventManager"/> instance.</returns>
    public EventManager ManageWithAttribute<TAttribute>(Assembly assembly)
        where TAttribute : Attribute
    {
        this.Log.D($"[EventManager]: Gathering events with {nameof(TAttribute)}...");
        this.ManageImplicitly(assembly, t => t.HasAttribute<TAttribute>());
        return this;
    }

    /// <summary>Disposes the <paramref name="event"/> instance and removes it from the cache.</summary>
    /// <param name="event">An <see cref="IManagedEvent"/> instance.</param>
    public void Unmanage(IManagedEvent @event)
    {
        if (!this._eventCache.TryGetValue(@event.GetType(), out var managed) || !ReferenceEquals(managed, @event))
        {
            this.Log.D($"[EventManager]:{@event.GetType().Name} was not being managed.");
            return;
        }

        @event.Dispose();
        this._eventCache.Remove(@event.GetType());
        this.Log.D($"[EventManager]: No longer managing {@event.GetType().Name}.");
    }

    /// <summary>Disposes an event of type <typeparamref name="TEvent"/>, if any exist.</summary>
    /// <typeparam name="TEvent">A <see cref="IManagedEvent"/> type to unmanage.</typeparam>
    public void Unmanage<TEvent>()
        where TEvent : IManagedEvent
    {
        this.Get<TEvent>()?.Unmanage();
    }

    /// <summary>Disposes all <see cref="IManagedEvent"/> instances and clear the event cache.</summary>
    public void UnmanageAll()
    {
        this._eventCache.ForEach(pair => pair.Value.Dispose());
        this._eventCache.Clear();
        this.Log.D("[EventManager]: No longer managing any events.");
    }

    /// <summary>Disposes all <see cref="IManagedEvent"/> instances belonging to the specified namespace and removes them from the cache.</summary>
    /// <param name="namespace">The desired namespace.</param>
    public void UnmanageNamespace(string @namespace)
    {
        var toUnmanage = this.GetAllForNamespace(@namespace).ToList();
        toUnmanage.ForEach(this.Unmanage);
        this.Log.D($"[EventManager]: No longer managing events in {@namespace}.");
    }

    /// <summary>Disposes all <see cref="IManagedEvent"/> instances with the specified attribute type.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    public void UnmanageWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        var toUnmanage = this.GetAllWithAttribute<TAttribute>().ToList();
        toUnmanage.ForEach(this.Unmanage);
        this.Log.D($"[EventManager]: No longer managing events with {nameof(TAttribute)}.");
    }

    /// <summary>Enable a single <see cref="IManagedEvent"/>.</summary>
    /// <param name="eventType">A <see cref="IManagedEvent"/> type to enable.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    public bool Enable(Type eventType)
    {
        if (this.GetOrCreate(eventType)?.Enable() == true)
        {
            this.Log.D($"[EventManager]: Enabled {eventType.Name}.");
            return true;
        }

        this.Log.D($"[EventManager]: {eventType.Name} was not enabled.");
        return false;
    }

    /// <summary>Enables the specified <see cref="IManagedEvent"/> types.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to enable.</param>
    public void Enable(params Type[] eventTypes)
    {
        foreach (var t in eventTypes)
        {
            this.Enable(t);
        }
    }

    /// <summary>Enable a single <see cref="IManagedEvent"/>.</summary>
    /// <typeparam name="TEvent">A <see cref="IManagedEvent"/> type to enable.</typeparam>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    public bool Enable<TEvent>()
        where TEvent : IManagedEvent
    {
        return this.Enable(typeof(TEvent));
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <param name="eventType">A <see cref="IManagedEvent"/> type to enable.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    public bool EnableForScreen(int screenId, Type eventType)
    {
        if (this.GetOrCreate(eventType)?.EnableForScreen(screenId) == true)
        {
            this.Log.D($"[EventManager]: Enabled {eventType.Name}.");
            return true;
        }

        this.Log.D($"[EventManager]: {eventType.Name} was not enabled.");
        return false;
    }

    /// <summary>Enables the specified <see cref="IManagedEvent"/> types for the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to enable.</param>
    public void EnableForScreen(int screenId, params Type[] eventTypes)
    {
        foreach (var t in eventTypes)
        {
            this.EnableForScreen(screenId, t);
        }
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">A <see cref="IManagedEvent"/> type to enable.</typeparam>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    public bool EnableForScreen<TEvent>(int screenId)
        where TEvent : IManagedEvent
    {
        return this.EnableForScreen(screenId, typeof(TEvent));
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <param name="eventType">A <see cref="IManagedEvent"/> type to enable.</param>
    public void EnableForAllScreens(Type eventType)
    {
        this.GetOrCreate(eventType)?.EnableForAllScreens();
        this.Log.D($"[EventManager]: Enabled {eventType.Name} for all screens.");
    }

    /// <summary>Enables the specified <see cref="IManagedEvent"/> types for the specified screen.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to enable.</param>
    public void EnableForAllScreens(params Type[] eventTypes)
    {
        foreach (var t in eventTypes)
        {
            this.EnableForAllScreens(t);
        }
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to enable.</typeparam>
    public void EnableForAllScreens<TEvent>()
        where TEvent : IManagedEvent
    {
        this.EnableForAllScreens(typeof(TEvent));
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/>.</summary>
    /// <param name="eventType">A <see cref="IManagedEvent"/> type to disable.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    public bool Disable(Type eventType)
    {
        if (this.GetOrCreate(eventType)?.Disable() == true)
        {
            this.Log.D($"[EventManager]: Disabled {eventType.Name}.");
            return true;
        }

        this.Log.D($"[EventManager]: {eventType.Name} was not disabled.");
        return false;
    }

    /// <summary>Disables the specified <see cref="IManagedEvent"/>s events.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to disable.</param>
    public void Disable(params Type[] eventTypes)
    {
        foreach (var t in eventTypes)
        {
            this.Disable(t);
        }
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/>.</summary>
    /// <typeparam name="TEvent">A <see cref="IManagedEvent"/> type to disable.</typeparam>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    public bool Disable<TEvent>()
        where TEvent : IManagedEvent
    {
        return this.Disable(typeof(TEvent));
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <param name="eventType">A <see cref="IManagedEvent"/> type to disable.</param>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    public bool DisableForScreen(Type eventType, int screenId)
    {
        if (this.GetOrCreate(eventType)?.DisableForScreen(screenId) == true)
        {
            this.Log.D($"[EventManager]: Disabled {eventType.Name}.");
            return true;
        }

        this.Log.D($"[EventManager]: {eventType.Name} was not disabled.");
        return false;
    }

    /// <summary>Disables the specified <see cref="IManagedEvent"/>s for the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to disable.</param>
    public void DisableForScreen(int screenId, params Type[] eventTypes)
    {
        foreach (var t in eventTypes)
        {
            this.DisableForScreen(t, screenId);
        }
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to disable.</typeparam>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    public bool DisableForScreen<TEvent>(int screenId)
        where TEvent : IManagedEvent
    {
        return this.DisableForScreen(typeof(TEvent), screenId);
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <param name="eventType">A <see cref="IManagedEvent"/> type to disable.</param>
    public void DisableForAllScreens(Type eventType)
    {
        this.GetOrCreate(eventType)?.DisableForAllScreens();
        this.Log.D($"[EventManager]: Enabled {eventType.Name} for all screens.");
    }

    /// <summary>Disables the specified <see cref="IManagedEvent"/>s for the specified screen.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to disable.</param>
    public void DisableForAllScreens(params Type[] eventTypes)
    {
        foreach (var t in eventTypes)
        {
            this.DisableForAllScreens(t);
        }
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">A <see cref="IManagedEvent"/> type to disable.</typeparam>
    public void DisableForAllScreens<TEvent>()
        where TEvent : IManagedEvent
    {
        this.DisableForAllScreens(typeof(TEvent));
    }

    /// <summary>Enables all <see cref="IManagedEvent"/>s.</summary>
    public void EnableAll()
    {
        var count = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract)
            .Count(this.Enable);
        this.Log.D($"[EventManager]: Enabled {count} events.");
    }

    /// <summary>Disables all <see cref="IManagedEvent"/>s.</summary>
    public void DisableAll()
    {
        var count = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract)
            .Count(this.Disable);
        this.Log.D($"[EventManager]: Disabled {count} events.");
    }

    /// <summary>Enables all <see cref="IManagedEvent"/> types starting with attribute <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    public void EnableWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        var count = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract && t.HasAttribute<TAttribute>())
            .Count(this.Enable);
        this.Log.D($"[EventManager]: Enabled {count} events.");
    }

    /// <summary>Disables all <see cref="IManagedEvent"/> types starting with attribute <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    public void DisableWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        var count = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract && t.HasAttribute<TAttribute>())
            .Count(this.Disable);
        this.Log.D($"[EventManager]: Disabled {count} events.");
    }

    /// <summary>Resets the enabled status of all <see cref="IManagedEvent"/>s in the assembly for the current screen.</summary>
    public void Reset()
    {
        this._eventCache.ForEach(pair => pair.Value.Reset());
        this.Log.D("[EventManager]: Reset all managed events for the current screen.");
    }

    /// <summary>Resets the enabled status of all <see cref="IManagedEvent"/>s in the assembly for all screens.</summary>
    public void ResetForAllScreens()
    {
        this._eventCache.ForEach(pair => pair.Value.ResetForAllScreens());
        this.Log.D("[EventManager]: Reset all managed events for all screens.");
    }

    /// <summary>Gets the <see cref="IManagedEvent"/> instance of type <paramref name="eventType"/>.</summary>
    /// <param name="eventType">A type implementing <see cref="IManagedEvent"/>.</param>
    /// <returns>A <see cref="IManagedEvent"/> instance of the specified <paramref name="eventType"/> if one exists, otherwise <see langword="null"/>.</returns>
    public IManagedEvent? Get(Type eventType)
    {
        return this._eventCache.TryGetValue(eventType, out var got) ? got : null;
    }

    /// <summary>Gets the <see cref="IManagedEvent"/> instance of type <typeparamref name="TEvent"/>.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <returns>A <see cref="IManagedEvent"/> instance of type <typeparamref name="TEvent"/> if one exists, otherwise <see langword="null"/>.</returns>
    public IManagedEvent? Get<TEvent>()
        where TEvent : IManagedEvent
    {
        return this.Get(typeof(TEvent));
    }

    /// <summary>Enumerates all managed <see cref="IManagedEvent"/> instances declared in the specified <paramref name="namespace"/>.</summary>
    /// <param name="namespace">The desired namespace.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IManagedEvent"/>s.</returns>
    public IEnumerable<IManagedEvent> GetAllForNamespace(string @namespace)
    {
        return this._eventCache
            .Where(pair => pair.Key.Namespace?.Contains(@namespace) ?? false)
            .Select(pair => pair.Value);
    }

    /// <summary>Enumerates all managed <see cref="IManagedEvent"/> instances with the specified <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IManagedEvent"/>s.</returns>
    public IEnumerable<IManagedEvent> GetAllWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return this._eventCache
            .Where(pair => pair.Key.HasAttribute<TAttribute>())
            .Select(pair => pair.Value);
    }

    /// <summary>Determines whether the specified <see cref="IManagedEvent"/> type is enabled.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <returns><see langword="true"/> if the <see cref="IManagedEvent"/> is enabled for the local screen, otherwise <see langword="false"/>.</returns>
    public bool IsEnabled<TEvent>()
        where TEvent : IManagedEvent
    {
        return this._eventCache.TryGetValue(typeof(TEvent), out var @event) && @event.IsEnabled;
    }

    /// <summary>Determines whether the specified <see cref="IManagedEvent"/> type is enabled for a specific screen.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <param name="screenId">The screen ID.</param>
    /// <returns><see langword="true"/> if the <see cref="IManagedEvent"/> is enabled for the specified screen, otherwise <see langword="false"/>.</returns>
    public bool IsEnabledForScreen<TEvent>(int screenId)
        where TEvent : IManagedEvent
    {
        return this._eventCache.TryGetValue(typeof(TEvent), out var @event) && @event.IsEnabledForScreen(screenId);
    }

    /// <summary>Logs information about currently managed <see cref="IManagedEvent"/> types.</summary>
    [Conditional("DEBUG")]
    public void LogStats()
    {
        var message = $"[EventManager]: Currently managing {this._eventCache.Count()} events, of which:";
        message = this._eventCache
            .GroupBy(pair => pair.Key.BaseType!)
            .Aggregate(message, (current, group) => current + $"\n\t- {group.Count()} {group.Key.Name}");
        this.Log.D(message);
    }

    /// <summary>Instantiates and manages <see cref="IManagedEvent"/> classes using reflection.</summary>
    /// <param name="assembly">The assembly to search within.</param>
    /// <param name="predicate">An optional condition with which to limit the scope of managed <see cref="IManagedEvent"/>s.</param>
    private void ManageImplicitly(Assembly assembly, Func<Type, bool>? predicate = null)
    {
        predicate ??= _ => true;
        var eventTypes = AccessTools
            .GetTypesFromAssembly(assembly)
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract && predicate(t))
            .ToArray();

        this.Log.D($"[EventManager]: Found {eventTypes.Length} event classes that should be enabled.");
        if (eventTypes.Length == 0)
        {
            return;
        }

        this.Log.D("[EventManager]: Instantiating events....");
        foreach (var eventType in eventTypes)
        {
#if RELEASE
            var debugAttribute = eventType.GetCustomAttribute<DebugAttribute>();
            if (debugAttribute is not null)
            {
                continue;
            }
#endif

            var ignoreAttribute = eventType.GetCustomAttribute<ImplicitIgnoreAttribute>();
            if (ignoreAttribute is not null)
            {
                continue;
            }

            var modRequirementAttribute = eventType.GetCustomAttribute<ModRequirementAttribute>();
            if (modRequirementAttribute is not null)
            {
                if (!this._modRegistry.IsLoaded(modRequirementAttribute.UniqueId))
                {
                    this.Log.D(
                        $"[EventManager]: The target mod {modRequirementAttribute.UniqueId} is not loaded. {eventType.Name} will be ignored.");
                    continue;
                }

                if (!string.IsNullOrEmpty(modRequirementAttribute.Version) &&
                    this._modRegistry.Get(modRequirementAttribute.UniqueId)!.Manifest.Version.IsOlderThan(
                        modRequirementAttribute.Version))
                {
                    this.Log.W(
                        $"[EventManager]: The integration event {eventType.Name} will be ignored because the installed version of {modRequirementAttribute.UniqueId} is older than minimum supported version." +
                        $" Please update {modRequirementAttribute.UniqueId} in order to enable integrations with this mod.");
                    continue;
                }
            }

            this.GetOrCreate(eventType);
        }
    }

    /// <summary>Retrieves an existing event instance from the cache, or caches a new instance.</summary>
    /// <param name="eventType">A type implementing <see cref="IManagedEvent"/>.</param>
    /// <returns>The cached <see cref="IManagedEvent"/> instance, or <see langword="null"/> if one could not be created.</returns>
    private IManagedEvent? GetOrCreate(Type eventType)
    {
        if (this._eventCache.TryGetValue(eventType, out var instance))
        {
            return instance;
        }

        instance = this.Create(eventType);
        if (instance is null)
        {
            this.Log.W($"[EventManager]: Failed to create {eventType.Name}.");
            return null;
        }

        this._eventCache.Add(eventType, instance);
        this.Log.D($"[EventManager]: Now managing {eventType.Name}.");

        return instance;
    }

    /// <summary>Retrieves an existing event instance from the cache, or caches a new instance.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <returns>The cached <see cref="IManagedEvent"/> instance, or <see langword="null"/> if one could not be created.</returns>
    private IManagedEvent? GetOrCreate<TEvent>()
    {
        return this.GetOrCreate(typeof(TEvent));
    }

    /// <summary>Instantiates a new <see cref="IManagedEvent"/> instance of the specified <paramref name="eventType"/>.</summary>
    /// <param name="eventType">A type implementing <see cref="IManagedEvent"/>.</param>
    /// <returns>A <see cref="IManagedEvent"/> instance of the specified <paramref name="eventType"/>.</returns>
    private IManagedEvent? Create(Type eventType)
    {
        if (!eventType.IsAssignableTo(typeof(IManagedEvent)) || eventType.IsAbstract || eventType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                [this.GetType()],
                null) is null)
        {
            this.Log.E($"[EventManager]: {eventType.Name} is not a valid event type.");
            return null;
        }

#if RELEASE
        var debugAttribute = eventType.GetCustomAttribute<DebugAttribute>();
        if (debugAttribute is not null)
        {
            return null;
        }
#endif

        var implicitIgnoreAttribute = eventType.GetCustomAttribute<ImplicitIgnoreAttribute>();
        if (implicitIgnoreAttribute is not null)
        {
            this.Log.D($"[EventManager]: {eventType.Name} is marked to be ignored.");
            return null;
        }

        var modRequirementAttribute = eventType.GetCustomAttribute<ModRequirementAttribute>();
        if (modRequirementAttribute is null)
        {
            return (IManagedEvent)eventType
                .RequireConstructor(this.GetType())
                .Invoke([this]);
        }

        if (!this._modRegistry.IsLoaded(modRequirementAttribute.UniqueId))
        {
            this.Log.D(
                $"[EventManager]: The target mod {modRequirementAttribute.UniqueId} is not loaded. {eventType.Name} will be ignored.");
            return null;
        }

        if (string.IsNullOrEmpty(modRequirementAttribute.Version) ||
            !this._modRegistry.Get(modRequirementAttribute.UniqueId)!.Manifest.Version.IsOlderThan(
                modRequirementAttribute.Version))
        {
            return (IManagedEvent)eventType
                .RequireConstructor(this.GetType())
                .Invoke([this]);
        }

        this.Log.W(
            $"[EventManager]: The integration event {eventType.Name} will be ignored because the installed version of {modRequirementAttribute.UniqueId} is older than minimum supported version." +
            $" Please update {modRequirementAttribute.UniqueId} in order to enable integrations with this mod.");
        return null;
    }
}
