namespace DaLion.Shared.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Exceptions;

#endregion using directives

/// <summary>The base implementation for a mod integration.</summary>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
/// <typeparam name="TIntegration">The <see cref="ModIntegration{TIntegration}"/> type inheriting from this class.</typeparam>
internal abstract class ModIntegration<TIntegration> : IModIntegration
    where TIntegration : ModIntegration<TIntegration>
{
    // ReSharper disable once InconsistentNaming
    private static readonly Lazy<TIntegration?> _instance = new(CreateInstance);

    /// <summary>Initializes a new instance of the <see cref="ModIntegration{TIntegration}"/> class.</summary>
    /// <param name="registry">An API for fetching metadata about loaded mods.</param>
    protected ModIntegration(IModRegistry registry)
    {
        var modRequirement = this.GetType().GetCustomAttribute<ModRequirementAttribute>()!;
        this.ModId = modRequirement.UniqueId;
        this.ModName = modRequirement.Name ?? this.ModId;
        this.ModRegistry = registry;
        this.IsLoaded = true;
    }

    /// <summary>Gets the singleton <typeparamref name="TIntegration"/> instance for this <see cref="ModIntegration{TIntegration}"/>.</summary>
    public static TIntegration? Instance => _instance.Value;

    /// <summary>Gets a value indicating whether an instance has been created for the singleton <typeparamref name="TIntegration"/> class.</summary>
    [MemberNotNullWhen(true, "Instance")]
    public static bool IsValueCreated => _instance.IsValueCreated && _instance.Value is not null;

    /// <inheritdoc />
    public string ModName { get; }

    /// <inheritdoc />
    public string ModId { get; }

    /// <inheritdoc />
    public virtual bool IsLoaded { get; }

    /// <inheritdoc />
    public virtual bool IsRegistered { get; protected set; }

    /// <summary>Gets aPI for fetching metadata about loaded mods.</summary>
    protected IModRegistry ModRegistry { get; }

    /// <inheritdoc />
    bool IModIntegration.Register()
    {
        if (this.IsRegistered || !this.RegisterImpl())
        {
            return false;
        }

        this.IsRegistered = true;
        return true;
    }

    /// <inheritdoc cref="IModIntegration.Register"/>
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
        var modRequirementAttribute = typeof(TIntegration).GetCustomAttribute<ModRequirementAttribute>();
        if (modRequirementAttribute is null)
        {
            ThrowHelperExtensions.ThrowTypeInitializationException();
        }

        if (!ModHelper.ModRegistry.IsLoaded(modRequirementAttribute.UniqueId))
        {
            Log.T($"{modRequirementAttribute.Name} is not installed. The mod integration will not be initialized.");
            return null;
        }

        var manifest = ModHelper.ModRegistry.Get(modRequirementAttribute.UniqueId)?.Manifest;
        if (manifest is null)
        {
            Log.E($"Failed to get the manifest for {modRequirementAttribute.Name}. The mod integration will not be initialized.");
            return null;
        }

        if (!string.IsNullOrEmpty(modRequirementAttribute.Version) &&
            manifest.Version.IsOlderThan(modRequirementAttribute.Version))
        {
            Log.W(
                $"Installed version of {modRequirementAttribute.Name} is older than the minimum version required. The mod integration will not be initialized." +
                "Please update the mod to enable the integration." +
                $"\n\tInstalled version: {manifest.Version}\n\tRequired version: {modRequirementAttribute.Version}");
            return null;
        }

        var uniqueId = modRequirementAttribute.UniqueId;
        return ModHelper.ModRegistry.IsLoaded(uniqueId)
            ? (TIntegration?)Activator.CreateInstance(typeof(TIntegration), nonPublic: true)
            : null;
    }
}

/// <summary>The base implementation for a mod integration.</summary>
/// <typeparam name="TIntegration">The <see cref="ModIntegration{TIntegration}"/> type inheriting from this class.</typeparam>
/// <typeparam name="TApi">The API type.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Same class overload.")]
internal abstract class ModIntegration<TIntegration, TApi> : ModIntegration<TIntegration>
    where TIntegration : ModIntegration<TIntegration>
    where TApi : class
{
    /// <summary>Initializes a new instance of the <see cref="ModIntegration{TIntegration, TApi}"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    protected ModIntegration(IModRegistry modRegistry)
        : base(modRegistry)
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
