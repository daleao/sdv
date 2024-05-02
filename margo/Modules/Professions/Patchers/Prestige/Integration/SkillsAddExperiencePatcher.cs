namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige.Integration;

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
    internal SkillsAddExperiencePatcher()
    {
        this.Target = this.RequireMethod<SpaceCore.Skills>(nameof(SpaceCore.Skills.AddExperience));
    }

    #region harmony patches

    /// <summary>Patch to apply prestige exp multiplier to custom skills.</summary>
    [HarmonyPrefix]
    private static void SkillsAddExperiencePrefix(string skillName, ref int amt)
    {
        if (!ProfessionsModule.EnableSkillReset || !CustomSkill.Loaded.TryGetValue(skillName, out var skill) ||
            amt <= 0)
        {
            return;
        }

        amt = Math.Min(
            (int)(amt * skill.BaseExperienceMultiplier * skill.PrestigeExperienceMultiplier),
            skill.ExperienceToMaxLevel - skill.CurrentExp);
    }

    #endregion harmony patches
}
