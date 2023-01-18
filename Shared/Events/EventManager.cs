namespace DaLion.Shared.Events;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using StardewModdingAPI.Events;

#endregion using directives

/// <summary>
///     Instantiates and manages dynamic enabling and disabling of <see cref="IManagedEvent"/> classes in an
///     assembly or namespace.
/// </summary>
internal sealed class EventManager
{
    /// <summary>Cache of <see cref="IManagedEvent"/> instances by type.</summary>
    private readonly ConditionalWeakTable<Type, IManagedEvent> _eventCache = new();

    /// <inheritdoc cref="IModRegistry"/>
    private readonly IModRegistry _modRegistry;

    /// <summary>Initializes a new instance of the <see cref="EventManager"/> class.</summary>
    /// <param name="modEvents">The <see cref="IModEvents"/> API for the current mod.</param>
    /// <param name="modRegistry">API for fetching metadata about loaded mods.</param>
    internal EventManager(IModEvents modEvents, IModRegistry modRegistry)
    {
        this._modRegistry = modRegistry;
        this.ModEvents = modEvents;
    }

    /// <inheritdoc cref="IModEvents"/>
    internal IModEvents ModEvents { get; }

    /// <summary>Gets an enumerable of all <see cref="IManagedEvent"/>s instances.</summary>
    internal IEnumerable<IManagedEvent> Managed => this._eventCache.Select(pair => pair.Value);

    /// <summary>Gets an enumerable of all <see cref="IManagedEvent"/>s currently enabled for the local player.</summary>
    internal IEnumerable<IManagedEvent> Enabled => this.Managed.Where(e => e.IsEnabled);

    /// <summary>Enumerates all <see cref="IManagedEvent"/>s currently enabled for the specified screen.</summary>
    /// <param name="screenId">The screen ID.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> of enabled <see cref="IManagedEvent"/>s in the specified screen.</returns>
    internal IEnumerable<IManagedEvent> EnabledForScreen(int screenId)
    {
        return this.Managed
            .Where(e => e.IsEnabledForScreen(screenId));
    }

    /// <summary>Adds the <paramref name="event"/> instance to the cache.</summary>
    /// <param name="event">An <see cref="IManagedEvent"/> instance.</param>
    internal void Manage(IManagedEvent @event)
    {
        this._eventCache.Add(@event.GetType(), @event);
        Log.D($"[EventManager]: Now managing {@event.GetType().Name}.");
    }

    /// <summary>Implicitly manages <see cref="IManagedEvent"/> types in the assembly.</summary>
    internal void ManageAll()
    {
        Log.D("[EventManager]: Gathering all events...");
        this.ManageImplicitly();
    }

    /// <summary>Implicitly manages <see cref="IManagedEvent"/> types in the specified namespace.</summary>
    /// <param name="namespace">The desired namespace.</param>
    internal void ManageNamespace(string @namespace)
    {
        Log.D($"[EventManager]: Gathering events in {@namespace}...");
        this.ManageImplicitly(t => t.Namespace?.StartsWith(@namespace) == true);
    }

    /// <summary>Implicitly manages <see cref="IManagedEvent"/> types with the specified attribute type.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    internal void ManageWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        Log.D($"[EventManager]: Gathering events with {nameof(TAttribute)}...");
        this.ManageImplicitly(t => t.GetCustomAttribute<TAttribute>() is not null);
    }

    /// <summary>Disposes the <paramref name="event"/> instance and removes it from the cache.</summary>
    /// <param name="event">An <see cref="IManagedEvent"/> instance.</param>
    internal void Unmanage(IManagedEvent @event)
    {
        @event.Dispose();
        this._eventCache.Remove(@event.GetType());
        Log.D($"[EventManager]: No longer managing {@event.GetType().Name}.");
    }

    /// <summary>Disposes all <see cref="IManagedEvent"/> instances and clear the event cache.</summary>
    internal void UnmanageAll()
    {
        this._eventCache.ForEach(pair => pair.Value.Dispose());
        this._eventCache.Clear();
        Log.D("[EventManager]: No longer managing events.");
    }

    /// <summary>Disposes all <see cref="IManagedEvent"/> instances belonging to the specified namespace and removes them from the cache.</summary>
    /// <param name="namespace">The desired namespace.</param>
    internal void UnmanageNamespace(string @namespace)
    {
        var toUnmanage = this.GetAllForNamespace(@namespace).ToList();
        toUnmanage.ForEach(this.Unmanage);
        Log.D($"[EventManager]: No longer managing events in {@namespace}.");
    }

    /// <summary>Disposes all <see cref="IManagedEvent"/> instances with the specified attribute type.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    internal void UnmanageWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        var toUnmanage = this.GetAllWithAttribute<TAttribute>().ToList();
        toUnmanage.ForEach(this.Unmanage);
        Log.D($"[EventManager]: No longer managing events with {nameof(TAttribute)}.");
    }

    /// <summary>Enable a single <see cref="IManagedEvent"/>.</summary>
    /// <param name="type">A <see cref="IManagedEvent"/> type to enable.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    internal bool Enable(Type type)
    {
        if (this.GetOrCreate(type)?.Enable() == true)
        {
            Log.D($"[EventManager]: Enabled {type.Name}.");
            return true;
        }

        Log.D($"[EventManager]: {type.Name} was not enabled.");
        return false;
    }

    /// <summary>Enables the specified <see cref="IManagedEvent"/> types.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to enable.</param>
    internal void Enable(params Type[] eventTypes)
    {
        for (var i = 0; i < eventTypes.Length; i++)
        {
            this.Enable(eventTypes[i]);
        }
    }

    /// <summary>Enable a single <see cref="IManagedEvent"/>.</summary>
    /// <typeparam name="TEvent">AA <see cref="IManagedEvent"/> type to enable.</typeparam>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    internal bool Enable<TEvent>()
        where TEvent : IManagedEvent
    {
        return this.Enable(typeof(TEvent));
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <param name="type">A <see cref="IManagedEvent"/> type to enable.</param>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    internal bool EnableForScreen(Type type, int screenId)
    {
        if (this.GetOrCreate(type)?.EnableForScreen(screenId) == true)
        {
            Log.D($"[EventManager]: Enabled {type.Name}.");
            return true;
        }

        Log.D($"[EventManager]: {type.Name} was not enabled.");
        return false;
    }

    /// <summary>Enables the specified <see cref="IManagedEvent"/> types for the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to enable.</param>
    internal void EnableForScreen(int screenId, params Type[] eventTypes)
    {
        for (var i = 0; i < eventTypes.Length; i++)
        {
            this.EnableForScreen(eventTypes[i], screenId);
        }
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">A <see cref="IManagedEvent"/> type to enable.</typeparam>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    internal bool EnableForScreen<TEvent>(int screenId)
        where TEvent : IManagedEvent
    {
        return this.EnableForScreen(typeof(TEvent), screenId);
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <param name="type">A <see cref="IManagedEvent"/> type to enable.</param>
    internal void EnableForAllScreens(Type type)
    {
        this.GetOrCreate(type)?.EnableForAllScreens();
        Log.D($"[EventManager]: Enabled {type.Name} for all screens.");
    }

    /// <summary>Enables the specified <see cref="IManagedEvent"/> types for the specified screen.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to enable.</param>
    internal void EnableForAllScreens(params Type[] eventTypes)
    {
        for (var i = 0; i < eventTypes.Length; i++)
        {
            this.EnableForAllScreens(eventTypes[i]);
        }
    }

    /// <summary>Enables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to enable.</typeparam>
    internal void EnableForAllScreens<TEvent>()
        where TEvent : IManagedEvent
    {
        this.EnableForAllScreens(typeof(TEvent));
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/>.</summary>
    /// <param name="type">A <see cref="IManagedEvent"/> type to disable.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    internal bool Disable(Type type)
    {
        if (this.GetOrCreate(type)?.Disable() == true)
        {
            Log.D($"[EventManager]: Disabled {type.Name}.");
            return true;
        }

        Log.D($"[EventManager]: {type.Name} was not disabled.");
        return false;
    }

    /// <summary>Disables the specified <see cref="IManagedEvent"/>s events.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to disable.</param>
    internal void Disable(params Type[] eventTypes)
    {
        for (var i = 0; i < eventTypes.Length; i++)
        {
            this.Disable(eventTypes[i]);
        }
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/>.</summary>
    /// <typeparam name="TEvent">A <see cref="IManagedEvent"/> type to disable.</typeparam>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    internal bool Disable<TEvent>()
        where TEvent : IManagedEvent
    {
        return this.Disable(typeof(TEvent));
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <param name="type">A <see cref="IManagedEvent"/> type to disable.</param>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    internal bool DisableForScreen(Type type, int screenId)
    {
        if (this.GetOrCreate(type)?.DisableForScreen(screenId) == true)
        {
            Log.D($"[EventManager]: Disabled {type.Name}.");
            return true;
        }

        Log.D($"[EventManager]: {type.Name} was not disabled.");
        return false;
    }

    /// <summary>Disables the specified <see cref="IManagedEvent"/>s for the specified screen.</summary>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to disable.</param>
    internal void DisableForScreen(int screenId, params Type[] eventTypes)
    {
        for (var i = 0; i < eventTypes.Length; i++)
        {
            this.DisableForScreen(eventTypes[i], screenId);
        }
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">An <see cref="IManagedEvent"/> type to disable.</typeparam>
    /// <param name="screenId">A local peer's screen ID.</param>
    /// <returns><see langword="true"/> if the event's enabled status was changed, otherwise <see langword="false"/>.</returns>
    internal bool DisableForScreen<TEvent>(int screenId)
        where TEvent : IManagedEvent
    {
        return this.DisableForScreen(typeof(TEvent), screenId);
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <param name="type">A <see cref="IManagedEvent"/> type to disable.</param>
    internal void DisableForAllScreens(Type type)
    {
        this.GetOrCreate(type)?.DisableForAllScreens();
        Log.D($"[EventManager]: Enabled {type.Name} for all screens.");
    }

    /// <summary>Disables the specified <see cref="IManagedEvent"/>s for the specified screen.</summary>
    /// <param name="eventTypes">The <see cref="IManagedEvent"/> types to disable.</param>
    internal void DisableForAllScreens(params Type[] eventTypes)
    {
        for (var i = 0; i < eventTypes.Length; i++)
        {
            this.DisableForAllScreens(eventTypes[i]);
        }
    }

    /// <summary>Disables a single <see cref="IManagedEvent"/> for the specified screen.</summary>
    /// <typeparam name="TEvent">A <see cref="IManagedEvent"/> type to disable.</typeparam>
    internal void DisableForAllScreens<TEvent>()
        where TEvent : IManagedEvent
    {
        this.DisableForAllScreens(typeof(TEvent));
    }

    /// <summary>Enables all <see cref="IManagedEvent"/>s.</summary>
    internal void EnableAll()
    {
        var count = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract)
            .Count(this.Enable);
        Log.D($"[EventManager]: Enabled {count} events.");
    }

    /// <summary>Disables all <see cref="IManagedEvent"/>s.</summary>
    internal void DisableAll()
    {
        var count = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract)
            .Count(this.Disable);
        Log.D($"[EventManager]: Disabled {count} events.");
    }

    /// <summary>Enables all <see cref="IManagedEvent"/> types starting with attribute <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    internal void EnableWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        var count = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract && t.GetCustomAttribute<TAttribute>() is not null)
            .Count(this.Enable);
        Log.D($"[EventManager]: Enabled {count} events.");
    }

    /// <summary>Disables all <see cref="IManagedEvent"/> types starting with attribute <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
    internal void DisableWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        var count = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract && t.GetCustomAttribute<TAttribute>() is not null)
            .Count(this.Disable);
        Log.D($"[EventManager]: Disabled {count} events.");
    }

    /// <summary>Resets the enabled status of all <see cref="IManagedEvent"/>s in the assembly for the current screen.</summary>
    internal void Reset()
    {
        this._eventCache.ForEach(pair => pair.Value.Reset());
        Log.D("[EventManager]: Reset all managed events for the current screen.");
    }

    /// <summary>Resets the enabled status of all <see cref="IManagedEvent"/>s in the assembly for all screens.</summary>
    internal void ResetForAllScreens()
    {
        this._eventCache.ForEach(pair => pair.Value.ResetForAllScreens());
        Log.D("[EventManager]: Reset all managed events for all screens.");
    }

    /// <summary>Enumerates all managed <see cref="IManagedEvent"/> instances declared in the specified <paramref name="namespace"/>.</summary>
    /// <param name="namespace">The desired namespace.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IManagedEvent"/>s.</returns>
    internal IEnumerable<IManagedEvent> GetAllForNamespace(string @namespace)
    {
        return this._eventCache
            .Where(pair => pair.Key.Namespace?.StartsWith(@namespace) ?? false)
            .Select(pair => pair.Value);
    }

    /// <summary>Enumerates all managed <see cref="IManagedEvent"/> instances with the specified <typeparamref name="TAttribute"/>.</summary>
    /// <typeparam name="TAttribute">An <see cref="Attribute"/> type.</typeparam>
    /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IManagedEvent"/>s.</returns>
    internal IEnumerable<IManagedEvent> GetAllWithAttribute<TAttribute>()
        where TAttribute : Attribute
    {
        return this._eventCache
            .Where(pair => pair.Key.GetCustomAttribute<TAttribute>() is not null)
            .Select(pair => pair.Value);
    }

    /// <summary>Determines whether the specified <see cref="IManagedEvent"/> type is enabled.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <returns><see langword="true"/> if the <see cref="IManagedEvent"/> is enabled for the local screen, otherwise <see langword="false"/>.</returns>
    internal bool IsEnabled<TEvent>()
        where TEvent : IManagedEvent
    {
        return this._eventCache.TryGetValue(typeof(TEvent), out var @event) && @event.IsEnabled;
    }

    /// <summary>Determines whether the specified <see cref="IManagedEvent"/> type is enabled for a specific screen.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <param name="screenId">The screen ID.</param>
    /// <returns><see langword="true"/> if the <see cref="IManagedEvent"/> is enabled for the specified screen, otherwise <see langword="false"/>.</returns>
    internal bool IsEnabledForScreen<TEvent>(int screenId)
        where TEvent : IManagedEvent
    {
        return this._eventCache.TryGetValue(typeof(TEvent), out var @event) && @event.IsEnabledForScreen(screenId);
    }

    /// <summary>Instantiates and manages <see cref="IManagedEvent"/> classes using reflection.</summary>
    /// <param name="predicate">An optional condition with which to limit the scope of managed <see cref="IManagedEvent"/>s.</param>
    private void ManageImplicitly(Func<Type, bool>? predicate = null)
    {
        predicate ??= t => true;
        var eventTypes = AccessTools
            .GetTypesFromAssembly(Assembly.GetAssembly(typeof(IManagedEvent)))
            .Where(t => t.IsAssignableTo(typeof(IManagedEvent)) && !t.IsAbstract && predicate(t) &&
                        // event classes may or not have the required internal parameterized constructor accepting only the manager instance, depending on whether they are SMAPI or mod-handled
                        // we only want to construct SMAPI events at this point, so we filter out the rest
                        t.GetConstructor(
                            BindingFlags.Instance | BindingFlags.NonPublic,
                            null,
                            new[] { this.GetType() },
                            null) is not null &&
                        (t.IsAssignableToAnyOf(typeof(GameLaunchedEvent), typeof(FirstSecondUpdateTickedEvent)) ||
                         t.GetCustomAttribute<AlwaysEnabledEventAttribute>() is not null ||
                         t.GetProperty(nameof(IManagedEvent.IsEnabled))?.DeclaringType == t))
            .ToArray();

        Log.D($"[EventManager]: Found {eventTypes.Length} event classes that should be enabled.");
        if (eventTypes.Length == 0)
        {
            return;
        }

        Log.D("[EventManager]: Instantiating events....");
        for (var i = 0; i < eventTypes.Length; i++)
        {
            var type = eventTypes[i];
#if RELEASE
            var debugAttribute = type.GetCustomAttribute<DebugAttribute>();
            if (debugAttribute is not null)
            {
                continue;
            }
#endif

            var deprecatedAttr = type.GetCustomAttribute<ImplicitIgnoreAttribute>();
            if (deprecatedAttr is not null)
            {
                continue;
            }

            var requiresModAttribute = type.GetCustomAttribute<RequiresModAttribute>();
            if (requiresModAttribute is not null)
            {
                if (!this._modRegistry.IsLoaded(requiresModAttribute.UniqueId))
                {
                    Log.D(
                        $"[EventManager]: The target mod {requiresModAttribute.UniqueId} is not loaded. {type.Name} will be ignored.");
                    continue;
                }

                if (!string.IsNullOrEmpty(requiresModAttribute.Version) &&
                    this._modRegistry.Get(requiresModAttribute.UniqueId)!.Manifest.Version.IsOlderThan(
                        requiresModAttribute.Version))
                {
                    Log.W(
                        $"[EventManager]: The integration event {type.Name} will be ignored because the installed version of {requiresModAttribute.UniqueId} is older than minimum supported version." +
                        $" Please update {requiresModAttribute.UniqueId} in order to enable integrations with this mod.");
                    continue;
                }
            }

            this.GetOrCreate(type);
        }
    }

    /// <summary>Retrieves an existing event instance from the cache, or caches a new instance.</summary>
    /// <param name="type">A type implementing <see cref="IManagedEvent"/>.</param>
    /// <returns>The cached <see cref="IManagedEvent"/> instance, or <see langword="null"/> if one could not be created.</returns>
    private IManagedEvent? GetOrCreate(Type type)
    {
        if (this._eventCache.TryGetValue(type, out var instance))
        {
            return instance;
        }

        instance = this.Create(type);
        if (instance is null)
        {
            Log.E($"[EventManager]: Failed to create {type.Name}.");
            return null;
        }

        this._eventCache.Add(type, instance);
        Log.D($"[EventManager]: Now managing {type.Name}.");

        return instance;
    }

    /// <summary>Retrieves an existing event instance from the cache, or caches a new instance.</summary>
    /// <typeparam name="TEvent">A type implementing <see cref="IManagedEvent"/>.</typeparam>
    /// <returns>The cached <see cref="IManagedEvent"/> instance, or <see langword="null"/> if one could not be created.</returns>
    private IManagedEvent? GetOrCreate<TEvent>()
    {
        return this.GetOrCreate(typeof(TEvent));
    }

    /// <summary>Instantiates a new <see cref="IManagedEvent"/> instance of the specified <paramref name="type"/>.</summary>
    /// <param name="type">A type implementing <see cref="IManagedEvent"/>.</param>
    /// <returns>A <see cref="IManagedEvent"/> instance of the specified <paramref name="type"/>.</returns>
    private IManagedEvent? Create(Type type)
    {
        if (!type.IsAssignableTo(typeof(IManagedEvent)) || type.IsAbstract || type.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { this.GetType() },
                null) is null)
        {
            Log.E($"[EventManager]: {type.Name} is not a valid event type.");
            return null;
        }

#if RELEASE
        var debugAttribute = type.GetCustomAttribute<DebugAttribute>();
        if (debugAttribute is not null)
        {
            return null;
        }
#endif

        var implicitIgnoreAttribute = type.GetCustomAttribute<ImplicitIgnoreAttribute>();
        if (implicitIgnoreAttribute is not null)
        {
            Log.D($"[EventManager]: {type.Name} is will be ignored.");
            return null;
        }

        var requiresModAttribute = type.GetCustomAttribute<RequiresModAttribute>();
        if (requiresModAttribute is not null)
        {
            if (!this._modRegistry.IsLoaded(requiresModAttribute.UniqueId))
            {
                Log.D(
                    $"[EventManager]: The target mod {requiresModAttribute.UniqueId} is not loaded. {type.Name} will be ignored.");
                return null;
            }

            if (!string.IsNullOrEmpty(requiresModAttribute.Version) &&
                this._modRegistry.Get(requiresModAttribute.UniqueId)!.Manifest.Version.IsOlderThan(
                    requiresModAttribute.Version))
            {
                Log.W(
                    $"[EventManager]: The integration event {type.Name} will be ignored because the installed version of {requiresModAttribute.UniqueId} is older than minimum supported version." +
                    $" Please update {requiresModAttribute.UniqueId} in order to enable integrations with this mod.");
                return null;
            }
        }

        return (IManagedEvent)type
            .GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { this.GetType() },
                null)!
            .Invoke(new object?[] { this });
    }
}
