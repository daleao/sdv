﻿namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using System.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGainExperiencePatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGainExperiencePatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerGainExperiencePatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.gainExperience));
    }

    #region harmony patches

    /// <summary>Patch to increase skill experience after each prestige + gate at level 10 until full prestige.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [UsedImplicitly]
    private static bool FarmerGainExperiencePrefix(Farmer __instance, int which, ref int howMuch)
    {
        if ((!ShouldEnableSkillReset && !ShouldEnablePrestigeLevels) || which == Farmer.luckSkill)
        {
            return true; // run original logic
        }

        if (howMuch <= 0)
        {
            return false; // don't run original logic
        }

        if (!__instance.IsLocalPlayer && Game1.IsServer)
        {
            __instance.queueMessage(17, Game1.player, which, howMuch);
            return false; // don't run original logic
        }

        try
        {
            var skill = Skill.FromValue(which);
            howMuch = Math.Max((int)(howMuch * skill.BaseExperienceMultiplier * ((ISkill)skill).BonusExperienceMultiplier), 1);
            if (((skill.CurrentLevel == 10 && !skill.CanGainPrestigeLevels()) || skill.CurrentLevel == 20) &&
                Skill.List.All(s => s.CurrentLevel >= 10))
            {
                var old = MasteryTrackerMenu.getCurrentMasteryLevel();
                Game1.stats.Increment("MasteryExp", howMuch);
                if (MasteryTrackerMenu.getCurrentMasteryLevel() <= old)
                {
                    return false; // don't run original logic
                }

                Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:Mastery_newlevel"));
                Game1.playSound("newArtifact");
                return false; // don't run original logic
            }

            if (skill.CurrentLevel == 10 && !skill.CanGainPrestigeLevels())
            {
                return false; // don't run original logic;
            }

            var newLevel = Math.Min(
                Farmer.checkForLevelGain(skill.CurrentExp, skill.CurrentExp + howMuch),
                skill.MaxLevel);
            if (newLevel <= skill.CurrentLevel)
            {
                skill.AddExperience(howMuch);
                return false; // don't run original logic
            }

            skill.AddExperience(howMuch);
            skill.SetLevel(newLevel);
            Game1.showGlobalMessage(Game1.content.LoadString("Strings\\1_6_Strings:NewIdeas"));
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
