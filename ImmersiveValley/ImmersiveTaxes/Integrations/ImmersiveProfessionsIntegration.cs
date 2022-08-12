namespace DaLion.Stardew.Taxes.Integrations;

#region using directives

using Common.Integrations;
using Common.Integrations.WalkOfLife;

#endregion using directives

internal sealed class ImmersiveProfessionsIntegration : BaseIntegration<IImmersiveProfessionsAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public ImmersiveProfessionsIntegration(IModRegistry modRegistry)
        : base("Immersive Professions", "DaLion.ImmersiveProfessions", "4.0.0", modRegistry) { }

    /// <summary>Cache the Immersive Professions API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.ProfessionsApi = ModApi;
    }
}