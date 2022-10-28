namespace DaLion.Redux.Professions.Integrations;

#region using directives

using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.LuckSkill;

#endregion using directives

internal sealed class LuckSkillIntegration : BaseIntegration<ILuckSkillApi>
{
    /// <summary>Initializes a new instance of the <see cref="LuckSkillIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    public LuckSkillIntegration(IModRegistry modRegistry)
        : base("LuckSkill", "spacechase0.LuckSkill", "1.2.3", modRegistry)
    {
    }

    /// <summary>Cache the Luck Skill API.</summary>
    public void Register()
    {
        this.AssertLoaded();
        Redux.Integrations.LuckSkillApi = this.ModApi;
    }
}
