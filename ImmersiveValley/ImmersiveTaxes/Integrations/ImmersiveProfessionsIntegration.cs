namespace DaLion.Stardew.Taxes.Integrations;

#region using directives

using System;
using StardewModdingAPI;

using Common.Integrations;

#endregion using directives

internal class ImmersiveProfessionsIntegration : BaseIntegration<IImmersiveProfessionsAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    /// <param name="log">Encapsulates monitoring and logging.</param>
    public ImmersiveProfessionsIntegration(IModRegistry modRegistry, Action<string, LogLevel> log)
        : base("Immersive Professions", "DaLion.ImmersiveProfessions", "4.0.0", modRegistry, log)
    {
    }

    /// <summary>Register the ring recipe provider.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.ProfessionsAPI = ModApi;
    }
}