namespace DaLion.Shared.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared.Attributes;

#endregion using directives

/// <summary>The base implementation for a mod integration.</summary>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
/// <typeparam name="TIntegration">The <see cref="ModIntegration{TIntegration}"/> type inheriting from this class.</typeparam>
public abstract class ModIntegration<TIntegration> : IModIntegration
    where TIntegration : ModIntegration<TIntegration>
{
    // ReSharper disable once InconsistentNaming
    private static readonly Lazy<TIntegration?> _instance = new(CreateInstance);

    /// <summary>Initializes a new instance of the <see cref="ModIntegration{TIntegration}"/> class.</summary>
    /// <param name="uniqueId">The mod's unique ID.</param>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="registry">An API for fetching metadata about loaded mods.</param>
    protected ModIntegration(string uniqueId, string? name, string? minVersion, IModRegistry registry)
    {
        this.ModId = uniqueId;
        this.ModName = name ?? uniqueId;
        this.ModRegistry = registry;

        var manifest = registry.Get(this.ModId)?.Manifest;
        if (manifest is null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(minVersion) && manifest.Version.IsOlderThan(minVersion))
        {
            Log.W(
                $"Integration for {name} will not be initialized because the installed version is older than the minimum version required." +
                $"\n\tInstalled version: {manifest.Version}\n\tMinimum version: {minVersion}");
            return;
        }

        this.IsLoaded = true;
    }

    /// <summary>Gets the singleton <typeparamref name="TIntegration"/> instance for this <see cref="ModIntegration{TIntegration}"/>.</summary>
    public static TIntegration? Instance => _instance.Value;

    /// <summary>Gets a value indicating whether an instance has been created for the singleton <typeparamref name="TIntegration"/> class.</summary>
    public static bool IsValueCreated => _instance.IsValueCreated && _instance.Value is not null;

    /// <inheritdoc />
    public string ModName { get; }

    /// <inheritdoc />
    public string ModId { get; }

    /// <inheritdoc />
    public virtual bool IsLoaded { get; }

    /// <summary>Gets or sets a value indicating whether the integration has been registered.</summary>
    public virtual bool IsRegistered { get; protected set; }

    /// <summary>Gets aPI for fetching metadata about loaded mods.</summary>
    protected IModRegistry ModRegistry { get; }

    /// <summary>Registers the integration and performs initial setup.</summary>
    internal void Register()
    {
        if (!this.IsRegistered && (this.IsRegistered = this.RegisterImpl()))
        {
            Log.T($"[Integrations]: The {this.ModName} integration has been registered.");
            return;
        }

        if (this.IsRegistered)
        {
            Log.T($"[Integrations]: The {this.ModName} integration is already registered.");
            return;
        }

        Log.W(
            $"[Integrations]: The {this.ModName} integration could not be registered. Some mod features have been disabled or will not work correctly.");
    }

    /// <inheritdoc cref="Register"/>
    /// <returns><see langword="true"/> if the registration was successful, otherwise <see langword="false"/>.</returns>
    protected virtual bool RegisterImpl()
    {
        return this.IsLoaded;
    }

    /// <summary>Try to get an API for the mod.</summary>
    /// <typeparam name="TApi">The API type.</typeparam>
    /// <param name="api">The API instance.</param>
    /// <returns><see langword="true"/> if an api was retrieved, otherwise <see langword="false"/>.</returns>
    protected bool TryGetApi<TApi>([NotNullWhen(true)] out TApi? api)
        where TApi : class
    {
        api = this.ModRegistry.GetApi<TApi>(this.ModId);
        return api is not null;
    }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    protected virtual void AssertLoaded()
    {
        if (!this.IsLoaded)
        {
            ThrowHelper.ThrowInvalidOperationException($"The {this.ModName} integration isn't loaded.");
        }
    }

    /// <summary>Assert that the integration is registered.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't registered.</exception>
    protected virtual void AssertRegistered()
    {
        if (!this.IsRegistered)
        {
            ThrowHelper.ThrowInvalidOperationException($"The {this.ModName} integration isn't registered.");
        }
    }

    private static TIntegration? CreateInstance()
    {
        var requiresModAttribute = typeof(TIntegration).GetCustomAttribute<RequiresModAttribute>();
        if (requiresModAttribute is null)
        {
            return null;
        }

        var uniqueId = requiresModAttribute.UniqueId;
        return ModHelper.ModRegistry.IsLoaded(uniqueId)
            ? (TIntegration?)Activator.CreateInstance(typeof(TIntegration), nonPublic: true)
            : null;
    }
}

/// <summary>The base implementation for a mod integration.</summary>
/// <typeparam name="TIntegration">The <see cref="ModIntegration{TIntegration}"/> type inheriting from this class.</typeparam>
/// <typeparam name="TApi">The API type.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Same class overload.")]
public abstract class ModIntegration<TIntegration, TApi> : ModIntegration<TIntegration>
    where TIntegration : ModIntegration<TIntegration>
    where TApi : class
{
    /// <summary>Initializes a new instance of the <see cref="ModIntegration{TIntegration, TApi}"/> class.</summary>
    /// <param name="uniqueId">The mod's unique ID.</param>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    protected ModIntegration(string uniqueId, string? name, string? minVersion, IModRegistry modRegistry)
        : base(uniqueId, name, minVersion, modRegistry)
    {
        if (base.IsLoaded && this.TryGetApi<TApi>(out var api))
        {
            this.ModApi = api;
            this.IsLoaded = true;
        }
        else
        {
            Log.W($"Failed to get api from {this.ModName}.");
        }
    }

    /// <summary>Gets the mod's public API.</summary>
    public TApi? ModApi { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(ModApi))]
    public override bool IsLoaded { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(ModApi))]
    public override bool IsRegistered { get; protected set; }

    /// <inheritdoc />
    [MemberNotNull(nameof(ModApi))]
    protected override void AssertLoaded()
    {
        if (!this.IsLoaded)
        {
            ThrowHelper.ThrowInvalidOperationException($"The {this.ModName} integration isn't loaded.");
        }
    }

    /// <inheritdoc />
    [MemberNotNull(nameof(ModApi))]
    protected override void AssertRegistered()
    {
        if (!this.IsRegistered)
        {
            ThrowHelper.ThrowInvalidOperationException($"The {this.ModName} integration isn't registered.");
        }
    }
}
