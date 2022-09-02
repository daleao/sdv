namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using System;

#endregion using directives

[UsedImplicitly, RequiresMod("spacechase0.SpaceCore")]
internal sealed class SkillsAddExperiencePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillsAddExperiencePatch()
    {
        Target = "SpaceCore.Skills".ToType().RequireMethod("AddExperience");
    }

    #region harmony patches

    /// <summary>Patch to apply prestige exp multiplier to custom skills.</summary>
    [HarmonyPrefix]
    private static void SkillsAddExperiencePrefix(Farmer farmer, string skillName, ref int amt)
    {
        if (!ModEntry.Config.EnablePrestige || !CustomSkill.LoadedSkills.TryGetValue(skillName, out var skill) ||
            amt < 0) return;

        amt = Math.Min((int)(amt * farmer.GetExperienceMultiplier(skill)), ISkill.VANILLA_EXP_CAP_I - skill.CurrentExp);
    }

    #endregion harmony patches
}