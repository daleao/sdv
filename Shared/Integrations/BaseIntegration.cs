namespace DaLion.Shared.Integrations;

#region using directives

using System.Diagnostics.CodeAnalysis;

#endregion using directives

/// <summary>The base implementation for a mod integration.</summary>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
public abstract class BaseIntegration : IModIntegration
{
    /// <summary>Initializes a new instance of the <see cref="BaseIntegration"/> class.</summary>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="modId">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    protected BaseIntegration(string name, string modId, string? minVersion, IModRegistry modRegistry)
    {
        this.ModName = name;
        this.ModId = modId;
        this.ModRegistry = modRegistry;

        var manifest = modRegistry.Get(this.ModId)?.Manifest;
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

    /// <summary>Gets a human-readable name for the mod.</summary>
    public string ModName { get; }

    /// <summary>Gets a value indicating whether the mod is available.</summary>
    public virtual bool IsLoaded { get; }

    /// <summary>Gets a value indicating whether the integration has been registered.</summary>
    public bool Registered { get; private set; }

    /// <summary>Gets the mod's unique ID.</summary>
    protected string ModId { get; }

    /// <summary>Gets aPI for fetching metadata about loaded mods.</summary>
    protected IModRegistry ModRegistry { get; }

    /// <summary>Registers the integration and performs initial setup.</summary>
    internal void Register()
    {
        this.AssertLoaded();
        this.RegisterImpl();
        this.Registered = true;
    }

    /// <inheritdoc cref="Register"/>
    protected virtual void RegisterImpl()
    {
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
}

/// <summary>The base implementation for a mod integration.</summary>
/// <typeparam name="TApi">The API type.</typeparam>
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Same class overload.")]
public abstract class BaseIntegration<TApi> : BaseIntegration
    where TApi : class
{
    /// <summary>Initializes a new instance of the <see cref="BaseIntegration{TApi}"/> class.</summary>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="modID">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    protected BaseIntegration(string name, string modID, string minVersion, IModRegistry modRegistry)
        : base(name, modID, minVersion, modRegistry)
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

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    [MemberNotNull(nameof(ModApi))]
    protected override void AssertLoaded()
    {
        if (!this.IsLoaded)
        {
            ThrowHelper.ThrowInvalidOperationException($"The {this.ModName} integration isn't loaded.");
        }
    }
}
