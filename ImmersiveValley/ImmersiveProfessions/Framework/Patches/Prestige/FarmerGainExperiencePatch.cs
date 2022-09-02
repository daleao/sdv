namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using DaLion.Common;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System;
using System.Reflection;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGainExperiencePatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerGainExperiencePatch()
    {
        Target = RequireMethod<Farmer>(nameof(Farmer.gainExperience));
        Prefix!.priority = Priority.LowerThanNormal;
    }

    #region harmony patches

    /// <summary>Patch to increase skill experience after each prestige + gate at level 10 until full prestige.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.LowerThanNormal)]
    private static bool FarmerGainExperiencePrefix(Farmer __instance, int which, ref int howMuch)
    {
        try
        {
            var skill = Skill.FromValue(which);
            if (which == Farmer.luckSkill && ModEntry.LuckSkillApi is null || howMuch <= 0)
                return false; // don't run original logic

            if (!__instance.IsLocalPlayer)
            {
                __instance.queueMessage(17, Game1.player, which, howMuch);
                return false; // don't run original logic
            }

            howMuch = (int)(howMuch * __instance.GetExperienceMultiplier(skill));
            var canGainPrestigeLevels = ModEntry.Config.EnablePrestige && __instance.HasAllProfessionsInSkill(skill) && skill != Farmer.luckSkill;
            var newLevel = Farmer.checkForLevelGain(skill.CurrentExp, skill.CurrentExp + howMuch);
            if (newLevel > 10 && !canGainPrestigeLevels) newLevel = 10;
            
            if (newLevel > skill.CurrentLevel)
            {
                for (var level = skill.CurrentLevel + 1; level <= newLevel; ++level)
                {
                    var point = new Point(which, level);
                    if (!Game1.player.newLevels.Contains(point))
                        Game1.player.newLevels.Add(point);
                }

                Game1.player.SetSkillLevel(skill, newLevel);
            }

            Game1.player.experiencePoints[skill] = Math.Min(skill.CurrentExp + howMuch,
                canGainPrestigeLevels ? ISkill.ExperienceByLevel[20] : ISkill.VANILLA_EXP_CAP_I);

            return false; // don't run original logic
        }
        catch (Exception ex)
        {
            Log.E($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}");
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}