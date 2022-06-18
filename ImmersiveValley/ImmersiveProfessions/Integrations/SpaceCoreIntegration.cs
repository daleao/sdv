#nullable enable
namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System;
using StardewModdingAPI;

using Common.Integrations;

#endregion using directives

internal class SpaceCoreIntegration : BaseIntegration<ISpaceCoreAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public SpaceCoreIntegration(IModRegistry modRegistry, Action<string, LogLevel> log)
        : base("SpaceCore", "spacechase0.SpaceCore", "1.8.3", modRegistry, log)
    {
    }

    /// <summary>Cache the SpaceCore API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.SpaceCoreApi = ModApi;
        ExtendedSpaceCoreAPI.Init();
    }
}