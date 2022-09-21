namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using System;
using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using DaLion.Stardew.Professions.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class SkillsAddExperiencePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="SkillsAddExperiencePatch"/> class.</summary>
    internal SkillsAddExperiencePatch()
    {
        this.Target = "SpaceCore.Skills"
            .ToType()
            .RequireMethod("AddExperience");
    }

    #region harmony patches

    /// <summary>Patch to apply prestige exp multiplier to custom skills.</summary>
    [HarmonyPrefix]
    private static void SkillsAddExperiencePrefix(Farmer farmer, string skillName, ref int amt)
    {
        if (!ModEntry.Config.EnablePrestige || !CustomSkill.Loaded.TryGetValue(skillName, out var skill) ||
            amt < 0)
        {
            return;
        }

        amt = Math.Min((int)(amt * farmer.GetExperienceMultiplier(skill)), ISkill.VanillaExpCap - skill.CurrentExp);
    }

    #endregion harmony patches
}
