namespace DaLion.Shared.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DaLion.Shared.Attributes;
using DaLion.Shared.Exceptions;
using Extensions.Reflection;

#endregion using directives

/// <summary>The base implementation for a mod integration.</summary>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
/// <typeparam name="TIntegration">The <see cref="ModIntegration{TIntegration}"/> type inheriting from this class.</typeparam>
public abstract class ModIntegration<TIntegration> : IModIntegration
    where TIntegration : ModIntegration<TIntegration>
{
    /// <summary>The singleton instance.</summary>
    // ReSharper disable once InconsistentNaming
    protected static readonly Lazy<TIntegration?> _instance = new(CreateInstance(ModHelper.ModRegistry));

    /// <summary>Initializes a new instance of the <see cref="ModIntegration{TIntegration}"/> class.</summary>
    /// <param name="registry">An API  for fetching metadata about loaded mods.</param>
    protected ModIntegration(IModRegistry registry)
    {
        var modRequirement = this.GetType().GetCustomAttribute<ModRequirementAttribute>()!; // guaranteed by CreateInstance
        this.ModId = modRequirement.UniqueId;
        this.ModName = modRequirement.Name;
        this.ModRegistry = registry;
        this.IsLoaded = true;
    }

    /// <summary>Gets the singleton <typeparamref name="TIntegration"/> instance for this <see cref="ModIntegration{TIntegration}"/>.</summary>
    public static TIntegration? Instance => _instance.Value;

    /// <summary>Gets a value indicating whether an instance has been created for the singleton <typeparamref name="TIntegration"/> class.</summary>
    [MemberNotNullWhen(true, nameof(Instance))]
    public static bool IsValueCreated => _instance is { IsValueCreated: true, Value: not null };

    /// <inheritdoc />
    public string ModName { get; }

    /// <inheritdoc />
    public string ModId { get; }

    /// <inheritdoc />
    public virtual bool IsLoaded { get; }

    /// <inheritdoc />
    public bool IsRegistered { get; protected set; }

    /// <summary>Gets aPI for fetching metadata about loaded mods.</summary>
    protected IModRegistry ModRegistry { get; }

    /// <inheritdoc />
    public bool Register()
    {
        return this.IsRegistered || (this.IsRegistered = this.RegisterImpl());
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

    private static TIntegration? CreateInstance(IModRegistry modRegistry)
    {
        var modRequirementAttribute = typeof(TIntegration).GetCustomAttribute<ModRequirementAttribute>();
        if (modRequirementAttribute is null)
        {
            ThrowHelperExtensions.ThrowTypeInitializationException();
        }

        var uniqueId = modRequirementAttribute.UniqueId;
        if (!modRegistry.IsLoaded(uniqueId))
        {
            Log.D($"{modRequirementAttribute.Name} is not installed. The mod integration will not be initialized.");
            return null;
        }

        var manifest = modRegistry.Get(uniqueId)?.Manifest;
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

        return (TIntegration?)typeof(TIntegration).RequireConstructor(Type.EmptyTypes).Invoke([]);
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

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(ModApi))]
    public override bool IsLoaded { get; }

    /// <summary>Gets the mod's public API.</summary>
    public TApi? ModApi { get; }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    [MemberNotNull(nameof(ModApi))]
    public void AssertLoaded()
    {
        if (!this.IsLoaded)
        {
            ThrowHelper.ThrowInvalidOperationException($"The {this.ModName} integration isn't loaded.");
        }
    }
}
