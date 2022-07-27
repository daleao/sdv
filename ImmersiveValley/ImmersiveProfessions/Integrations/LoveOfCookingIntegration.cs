namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using Common.Integrations;
using Common.Integrations.LoveOfCooking;

#endregion using directives

internal sealed class LoveOfCookingIntegration : BaseIntegration<ICookingSkillAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public LoveOfCookingIntegration(IModRegistry modRegistry)
        : base("LoveOfCooking", "blueberry.LoveOfCooking", "1.0.27", modRegistry) { }

    /// <summary>Cache the Love Of Cooking API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.CookingSkillApi = ModApi;
    }
}