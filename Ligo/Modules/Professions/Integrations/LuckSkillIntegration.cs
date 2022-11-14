namespace DaLion.Ligo.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.LuckSkill;

#endregion using directives

internal sealed class LuckSkillIntegration : BaseIntegration<ILuckSkillApi>
{
    /// <summary>Initializes a new instance of the <see cref="LuckSkillIntegration"/> class.</summary>
    /// <param name="modRegistry">An API for fetching metadata about loaded mods.</param>
    internal LuckSkillIntegration(IModRegistry modRegistry)
        : base("LuckSkill", "spacechase0.LuckSkill", "1.2.3", modRegistry)
    {
    }

    /// <summary>Instantiates and caches the <see cref="LuckSkill"/> instance.</summary>
    internal static void LoadLuckSkill()
    {
        if (SCSkill.Loaded.ContainsKey("spacechase0.LuckSkill"))
        {
            return;
        }

        var luckSkill = new LuckSkill();
        SCSkill.Loaded["spacechase0.LuckSkill"] = luckSkill;
        foreach (var profession in luckSkill.Professions)
        {
            SCProfession.Loaded[profession.Id] = (SCProfession)profession;
        }
    }

    /// <summary>Caches the Luck Skill API.</summary>
    internal void Register()
    {
        this.AssertLoaded();
        Ligo.Integrations.LuckSkillApi = this.ModApi;
    }
}
