namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Linq;
using Utility;

#endregion using directives

[UsedImplicitly, RequiresMod("spacesechase0.SpaceCore")]
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
        if (!ModEntry.Config.EnablePrestige || !ModEntry.CustomSkills.TryGetValue(skillName, out var skill) ||
            amt < 0) return;

        amt = Math.Min(
            (int)(amt * Math.Pow(1f + ModEntry.Config.BonusSkillExpPerReset,
                farmer.GetProfessionsForSkill(skill, true).Count())), Experience.VANILLA_CAP_I - skill.CurrentExp);
    }

    #endregion harmony patches
}