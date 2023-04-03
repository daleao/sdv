namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.LuckSkill;

#endregion using directives

[RequiresMod("spacechase0.LuckSkill", "Luck Skill", "1.2.3")]
internal sealed class LuckSkillIntegration : ModIntegration<LuckSkillIntegration, ILuckSkillApi>
{
    private LuckSkillIntegration()
        : base("spacechase0.LuckSkill", "Luck Skill", "1.2.3", ModHelper.ModRegistry)
    {
    }

    /// <summary>Instantiates and caches the <see cref="LuckSkill"/> instance.</summary>
    internal void LoadLuckSkill()
    {
        this.AssertLoaded();

        if (LuckSkill.Instance is not null)
        {
            return;
        }

        var luckSkill = new LuckSkill();
        Skill.Luck = luckSkill;
        SCSkill.Loaded["spacechase0.LuckSkill"] = luckSkill;
        Log.T($"[PROFS]: Successfully loaded the custom skill {this.ModId}.");
    }
}
