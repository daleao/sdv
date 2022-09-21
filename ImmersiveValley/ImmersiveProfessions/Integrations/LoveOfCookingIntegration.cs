namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using DaLion.Common.Integrations;
using DaLion.Common.Integrations.LoveOfCooking;

#endregion using directives

internal sealed class LoveOfCookingIntegration : BaseIntegration<ICookingSkillApi>
{
    /// <summary>Initializes a new instance of the <see cref="LoveOfCookingIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public LoveOfCookingIntegration(IModRegistry modRegistry)
        : base("LoveOfCooking", "blueberry.LoveOfCooking", "1.0.27", modRegistry)
    {
    }

    /// <summary>Cache the Love Of Cooking API.</summary>
    public void Register()
    {
        this.AssertLoaded();
        ModEntry.CookingSkillApi = this.ModApi;
    }
}
