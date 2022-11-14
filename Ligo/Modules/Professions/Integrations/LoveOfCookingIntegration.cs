namespace DaLion.Ligo.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.LoveOfCooking;

#endregion using directives

internal sealed class LoveOfCookingIntegration : BaseIntegration<ICookingSkillApi>
{
    /// <summary>Initializes a new instance of the <see cref="LoveOfCookingIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal LoveOfCookingIntegration(IModRegistry modRegistry)
        : base("LoveOfCooking", "blueberry.LoveOfCooking", "1.0.27", modRegistry)
    {
    }

    /// <summary>Cache the Love Of Cooking API.</summary>
    internal void Register()
    {
        this.AssertLoaded();
        Ligo.Integrations.CookingSkillApi = this.ModApi;
    }
}
