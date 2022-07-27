namespace DaLion.Common.Integrations;

#region using directives

using System;
using System.Diagnostics.CodeAnalysis;

#endregion using directives

/// <summary>The base implementation for a mod integration.</summary>
/// <remarks>Original code by <see href="https://github.com/Pathoschild">Pathoschild</see>.</remarks>
public abstract class BaseIntegration : IModIntegration
{
    #region accessors

    /// <summary>A human-readable name for the mod.</summary>
    public string ModName { get; }

    /// <summary>The mod's unique ID.</summary>
    protected string ModId { get; }

    /// <summary>API for fetching metadata about loaded mods.</summary>
    protected IModRegistry ModRegistry { get; }

    /// <summary>Whether the mod is available.</summary>
    public virtual bool IsLoaded { get; }

    #endregion accessors

    /// <summary>Construct an instance.</summary>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="modId">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    protected BaseIntegration(string name, string modId, string minVersion, IModRegistry modRegistry)
    {
        // init
        ModName = name;
        ModId = modId;
        ModRegistry = modRegistry;

        // validate mod
        var manifest = modRegistry.Get(ModId)?.Manifest;
        if (manifest is null) return;

        if (manifest.Version.IsOlderThan(minVersion))
        {
            Log.W(
                $"Detected {name} {manifest.Version}, but need {minVersion} or later. Disabled integration with this mod.");
            return;
        }

        IsLoaded = true;
    }

    /// <summary>Try to get an API for the mod.</summary>
    /// <typeparam name="TApi">The API type.</typeparam>
    /// <returns><see langword="true"> if an api was retreived, otherwise <see langword="false">.</returns>
    protected bool TryGetApi<TApi>([NotNullWhen(true)] out TApi? api) where TApi : class
    {
        api = ModRegistry.GetApi<TApi>(ModId);
        return api is not null;
    }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    protected virtual void AssertLoaded()
    {
        if (!IsLoaded) throw new InvalidOperationException($"The {ModName} integration isn't loaded.");
    }
}

/// <summary>The base implementation for a mod integration.</summary>
/// <typeparam name="TApi">The API type.</typeparam>
public abstract class BaseIntegration<TApi> : BaseIntegration where TApi : class
{
    #region accessors

    /// <summary>The mod's public API.</summary>
    public TApi? ModApi { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(true, nameof(ModApi))]
    public override bool IsLoaded => ModApi != null;

    #endregion accessors

    /// <summary>Construct an instance.</summary>
    /// <param name="name">A human-readable name for the mod.</param>
    /// <param name="modID">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    protected BaseIntegration(string name, string modID, string minVersion, IModRegistry modRegistry)
        : base(name, modID, minVersion, modRegistry)
    {
        if (base.IsLoaded && TryGetApi<TApi>(out var api))
            ModApi = api;
        else
            Log.W($"Failed to get api from {ModName}.");
    }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    [MemberNotNull(nameof(ModApi))]
    protected override void AssertLoaded()
    {
        if (!IsLoaded) throw new InvalidOperationException($"The {ModName} integration isn't loaded.");
    }
}