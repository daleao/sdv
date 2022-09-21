namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using System;
using System.Reflection;
using DaLion.Common;
using DaLion.Stardew.Professions.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGainExperiencePatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGainExperiencePatch"/> class.</summary>
    internal FarmerGainExperiencePatch()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.gainExperience));
        this.Prefix!.priority = Priority.LowerThanNormal;
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
            if ((which == Farmer.luckSkill && ModEntry.LuckSkillApi is null) || howMuch <= 0)
            {
                return false; // don't run original logic
            }

            if (!__instance.IsLocalPlayer)
            {
                __instance.queueMessage(17, Game1.player, which, howMuch);
                return false; // don't run original logic
            }

            howMuch = (int)(howMuch * __instance.GetExperienceMultiplier(skill));
            var canGainPrestigeLevels = ModEntry.Config.EnablePrestige && __instance.HasAllProfessionsInSkill(skill) &&
                                        skill != Farmer.luckSkill;
            var newLevel = Farmer.checkForLevelGain(skill.CurrentExp, skill.CurrentExp + howMuch);
            if (newLevel > 10 && !canGainPrestigeLevels)
            {
                newLevel = 10;
            }

            if (newLevel > skill.CurrentLevel)
            {
                for (var level = skill.CurrentLevel + 1; level <= newLevel; ++level)
                {
                    var point = new Point(which, level);
                    if (!Game1.player.newLevels.Contains(point))
                    {
                        Game1.player.newLevels.Add(point);
                    }
                }

                Game1.player.SetSkillLevel(skill, newLevel);
            }

            Game1.player.experiencePoints[skill] = Math.Min(
                skill.CurrentExp + howMuch,
                canGainPrestigeLevels ? ISkill.ExperienceByLevel[20] : ISkill.VanillaExpCap);

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
