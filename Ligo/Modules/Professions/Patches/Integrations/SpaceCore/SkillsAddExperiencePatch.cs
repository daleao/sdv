namespace DaLion.Ligo.Modules.Professions.Patches.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[Integration("spacechase0.SpaceCore")]
internal sealed class SkillsAddExperiencePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SkillsAddExperiencePatch"/> class.</summary>
    internal SkillsAddExperiencePatch()
    {
        this.Target = this.RequireMethod<SpaceCore.Skills>(nameof(SpaceCore.Skills.AddExperience));
    }

    #region harmony patches

    /// <summary>Patch to apply prestige exp multiplier to custom skills.</summary>
    [HarmonyPrefix]
    private static void SkillsAddExperiencePrefix(string skillName, ref int amt)
    {
        if (!ModEntry.Config.Professions.EnablePrestige || !SCSkill.Loaded.TryGetValue(skillName, out var skill) ||
            amt <= 0)
        {
            return;
        }

        amt = (int)(amt * skill.BaseExperienceMultiplier * skill.PrestigeExperienceMultiplier);
    }

    #endregion harmony patches
}
