namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;

using DaLion.Common;
using DaLion.Common.Harmony;
using Extensions;
using Utility;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGainExperiencePatch : BasePatch
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

            var canGainPrestigeLevels = ModEntry.Config.EnablePrestige && __instance.HasAllProfessionsInSkill(skill);
            
            howMuch = (int) (howMuch * ModEntry.Config.BaseSkillExpMultiplierPerSkill[which]);
            if (ModEntry.Config.EnablePrestige)
            {
                howMuch = (int) (howMuch * Math.Pow(1f + ModEntry.Config.BonusSkillExpPerReset,
                    __instance.GetProfessionsForSkill(skill, true).Count()));
            }

            var newLevel = Farmer.checkForLevelGain(skill.CurrentExp, skill.CurrentExp + howMuch);
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
                canGainPrestigeLevels ? Experience.PrestigeCap : Experience.VANILLA_CAP_I);

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