namespace DaLion.Professions.Framework.Patchers.Prestige.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class SkillsAddExperiencePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsAddExperiencePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SkillsAddExperiencePatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SCSkills>(nameof(SCSkills.AddExperience));
    }

    #region harmony patches

    /// <summary>Patch to apply prestige exp multiplier to custom skills.</summary>
    [HarmonyPrefix]
    private static void SkillsAddExperiencePrefix(string skillName, ref int amt)
    {
        if (!ShouldEnableSkillReset || !CustomSkill.Loaded.TryGetValue(skillName, out var skill) ||
            amt <= 0)
        {
            return;
        }

        amt = Math.Min(
            (int)(amt * skill.BaseExperienceMultiplier), // might need to add prestige bonus here later
            ((ISkill)skill).ExperienceToMaxLevel - skill.CurrentExp);
    }

    #endregion harmony patches
}
