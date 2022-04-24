namespace DaLion.Common.Stardew.Integrations;

#region using directives

using System;
using StardewModdingAPI;

#endregion using directives

/// <summary>The base implementation for a mod integration.</summary>
/// <remarks>Credit to <c>Pathoschild</c>.</remarks>
internal abstract class BaseIntegration : IModIntegration
{
    /// <summary>Construct an instance.</summary>
    /// <param name="label">A human-readable name for the mod.</param>
    /// <param name="modID">The mod's unique ID.</param>
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

    /// <summary>The mod's unique ID.</summary>
    protected string ModId { get; }

    /// <summary>API for fetching metadata about loaded mods.</summary>
    protected IModRegistry ModRegistry { get; }

    /// <summary>Encapsulates monitoring and logging.</summary>
    protected Action<string, LogLevel> Log { get; }

    /// <summary>A human-readable name for the mod.</summary>
    public string Label { get; }

    /// <summary>Whether the mod is available.</summary>
    public bool IsLoaded { get; protected set; }

    /// <summary>Get an API for the mod, and show a message if it can't be loaded.</summary>
    /// <typeparam name="TInterface">The API type.</typeparam>
    protected TInterface GetValidatedApi<TInterface>() where TInterface : class
    {
        var api = ModRegistry.GetApi<TInterface>(ModId);
        if (api is not null) return api;

        Log($"Detected {Label}, but couldn't fetch its API. Disabled integration with this mod.",
            LogLevel.Warn);
        return null;
    }

    /// <summary>Assert that the integration is loaded.</summary>
    /// <exception cref="InvalidOperationException">The integration isn't loaded.</exception>
    protected void AssertLoaded()
    {
        if (!IsLoaded) throw new InvalidOperationException($"The {Label} integration isn't loaded.");
    }
}