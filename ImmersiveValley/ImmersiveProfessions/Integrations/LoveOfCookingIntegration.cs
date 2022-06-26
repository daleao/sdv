namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using Common.Integrations;
using StardewModdingAPI;

#endregion using directives

internal class LoveOfCookingIntegration : BaseIntegration<ICookingSkillAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public LoveOfCookingIntegration(IModRegistry modRegistry)
        : base("LoveOfCooking", "blueberry.LoveOfCooking", "1.0.27", modRegistry)
    {
    }

    /// <summary>Cache the SpaceCore API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.CookingSkillApi = ModApi;
    }
}