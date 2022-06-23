namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using System;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common.Extensions.Reflection;
using DaLion.Common.Harmony;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsAddExperiencePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillsAddExperiencePatch()
    {
        try
        {
            Target = "SpaceCore.Skills".ToType().RequireMethod("AddExperience");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to apply prestige exp multiplier to custom skills.</summary>
    [HarmonyPrefix]
    private static void SkillsAddExperiencePrefix(Farmer farmer, string skillName, ref int amt)
    {
        if (!ModEntry.Config.EnablePrestige || !ModEntry.CustomSkills.TryGetValue(skillName, out var skill)) return;

        amt = (int) (amt * Math.Pow(1f + ModEntry.Config.BonusSkillExpPerReset,
            farmer.GetProfessionsForSkill(skill, true).Count()));
    }

    #endregion harmony patches
}