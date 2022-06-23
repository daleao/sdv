#nullable enable
namespace DaLion.Stardew.Professions.Integrations;

#region using directives

using StardewModdingAPI;

using Common.Integrations;

#endregion using directives

internal class LuckSkillIntegration : BaseIntegration<ILuckSkillAPI>
{
    /// <summary>Construct an instance.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public LuckSkillIntegration(IModRegistry modRegistry)
        : base("LuckSkill", "spacechase0.LuckSkill", "1.2.3", modRegistry)
    {
    }

    /// <summary>Cache the Luck Skill API.</summary>
    public void Register()
    {
        AssertLoaded();
        ModEntry.LuckSkillApi = ModApi;
        ExtendedLuckSkillAPI.Init();
    }
}