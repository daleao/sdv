#nullable enable
namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using System;
using StardewModdingAPI;

using Common.Integrations;

#endregion using directives

internal class LoveOfCookingIntegration : BaseIntegration<ICookingSkillAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public LoveOfCookingIntegration(IModRegistry modRegistry, Action<string, LogLevel> log)
        : base("LoveOfCooking", "blueberry.LoveOfCooking", "1.0.27", modRegistry, log)
    {
    }

    /// <summary>Cache the SpaceCore API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.CookingSkillApi = ModApi;
    }
}