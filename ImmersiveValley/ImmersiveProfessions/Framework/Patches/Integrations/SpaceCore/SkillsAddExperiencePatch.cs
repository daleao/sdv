namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using System;
using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using DaLion.Common.Extensions.Reflection;
using Extensions;

#endregion using directives

[UsedImplicitly]
internal class SkillsAddExperiencePatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillsAddExperiencePatch()
    {
        try
        {
            Original = "SpaceCore.Skills".ToType().RequireMethod("AddExperience");
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
        if (!ModEntry.Config.EnablePrestige) return;

        var theSkill = ModEntry.CustomSkills[skillName];
        amt = (int) (amt * Math.Pow(1f + ModEntry.Config.BonusSkillExpPerReset,
            farmer.NumberOfProfessionsInCustomSkill(theSkill, true)));
    }

    #endregion harmony patches
}