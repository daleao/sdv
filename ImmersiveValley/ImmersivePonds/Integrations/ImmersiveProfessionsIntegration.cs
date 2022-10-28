namespace DaLion.Stardew.Ponds.Integrations;

#region using directives

using DaLion.Common.Integrations;
using Common.Integrations.ImmersiveProfessions;

#endregion using directives

internal sealed class ImmersiveProfessionsIntegration : BaseIntegration<IImmersiveProfessionsApi>
{
    /// <summary>Initializes a new instance of the <see cref="ImmersiveProfessionsIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public ImmersiveProfessionsIntegration(IModRegistry modRegistry)
        : base("Immersive Professions", "DaLion.ImmersiveProfessions", "4.0.0", modRegistry)
    {
    }

    /// <summary>Cache the immersive professions api.</summary>
    public void Register()
    {
        this.AssertLoaded();
        ModEntry.ProfessionsApi = this.ModApi;
    }
}
