#nullable enable
namespace DaLion.Common.Integrations;

#region using directives

using System;
using System.Diagnostics.CodeAnalysis;
using StardewModdingAPI;

#endregion using directives

/// <summary>The base implementation for a mod integration.</summary>
/// <remarks>Credit to <c>Pathoschild</c>.</remarks>
public abstract class BaseIntegration : IModIntegration
{
    #region accessors

    /// <summary>The mod's unique ID.</summary>
    protected string ModId { get; }

    /// <summary>API for fetching metadata about loaded mods.</summary>
    protected IModRegistry ModRegistry { get; }

    /// <summary>Encapsulates monitoring and logging.</summary>
    protected Action<string, LogLevel> Log { get; }

    /// <summary>A human-readable name for the mod.</summary>
    public string Label { get; }

    /// <summary>Whether the mod is available.</summary>
    public virtual bool IsLoaded { get; }

    #endregion accessors


    /// <summary>Construct an instance.</summary>
    /// <param name="label">A human-readable name for the mod.</param>
    /// <param name="modId">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    protected BaseIntegration(string label, string modId, string minVersion, IModRegistry modRegistry,
        Action<string, LogLevel> log)
    {
        // init
        Label = label;
        ModId = modId;
        ModRegistry = modRegistry;
        Log = log;

        // validate mod
        var manifest = modRegistry.Get(ModId)?.Manifest;
        if (manifest is null) return;

        if (manifest.Version.IsOlderThan(minVersion))
        {
            Log(
                $"Detected {label} {manifest.Version}, but need {minVersion} or later. Disabled integration with this mod.",
                LogLevel.Warn);
            return;
        }

        IsLoaded = true;
    }

    /// <summary>Get an API for the mod, and show a message if it can't be loaded.</summary>
    /// <typeparam name="TApi">The API type.</typeparam>
    protected TApi? GetValidatedApi<TApi>() where TApi : class
    {
        var api = ModRegistry.GetApi<TApi>(ModId);
        if (api is not null) return api;

        Log($"Detected {Label}, but couldn't fetch its API. Disabled integration with this mod.",
            LogLevel.Warn);
        return null;
    }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    protected virtual void AssertLoaded()
    {
        if (!IsLoaded) throw new InvalidOperationException($"The {Label} integration isn't loaded.");
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
    /// <param name="label">A human-readable name for the mod.</param>
    /// <param name="modID">The mod's unique ID.</param>
    /// <param name="minVersion">The minimum version of the mod that's supported.</param>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="monitor">Encapsulates monitoring and logging.</param>
    protected BaseIntegration(string label, string modID, string minVersion, IModRegistry modRegistry, Action<string, LogLevel> log)
        : base(label, modID, minVersion, modRegistry, log)
    {
        if (base.IsLoaded) ModApi = GetValidatedApi<TApi>();
    }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    [MemberNotNull(nameof(ModApi))]
    protected override void AssertLoaded()
    {
        if (!IsLoaded) throw new InvalidOperationException($"The {Label} integration isn't loaded.");
    }
}