namespace DaLion.Stardew.Taxes.Integrations;

#region using directives

using Common.Integrations;
using StardewModdingAPI;

#endregion using directives

internal class ImmersiveProfessionsIntegration : BaseIntegration<IImmersiveProfessionsAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public ImmersiveProfessionsIntegration(IModRegistry modRegistry)
        : base("Immersive Professions", "DaLion.ImmersiveProfessions", "4.0.0", modRegistry)
    {
    }

    /// <summary>Register the ring recipe provider.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.ProfessionsAPI = ModApi;
    }
}