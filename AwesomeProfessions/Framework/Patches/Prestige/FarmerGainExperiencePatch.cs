using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

[UsedImplicitly]
internal class FarmerGainExperiencePatch : BasePatch
{
    private const int PRESTIGE_GATE_I = 15000;

    /// <summary>Construct an instance.</summary>
    internal FarmerGainExperiencePatch()
    {
        Original = RequireMethod<Farmer>(nameof(Farmer.gainExperience));
    }

    #region harmony patches

    /// <summary>Patch to increase skill experience after each prestige + gate at level 10 until full prestige.</summary>
    [HarmonyPrefix]
    private static bool FarmerGainExperiencePrefix(Farmer __instance, int which, ref int howMuch)
    {
        howMuch = (int) (howMuch * ModEntry.Config.BaseSkillExpMultiplier);

        if (!ModEntry.Config.EnablePrestige) return true; // run original logic

        try
        {
            var howMuchAdjusted = (int) (howMuch * Math.Pow(1f + ModEntry.Config.BonusSkillExpPerReset,
                __instance.NumberOfProfessionsInSkill(which, true)));
            switch (__instance.experiencePoints[which])
            {
                case >= PRESTIGE_GATE_I when !__instance.HasAllProfessionsInSkill(which):
                    return false; // don't run original logic
                case < PRESTIGE_GATE_I:
                    var leftUntilCutoff = PRESTIGE_GATE_I - __instance.experiencePoints[which];
                    howMuch = Math.Min(howMuchAdjusted, leftUntilCutoff);
                    return true; // run original logic
                default:
                    howMuch = howMuchAdjusted;
                    return true; // run original logic
            }
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Failed in {MethodBase.GetCurrentMethod()?.Name}:\n{ex}", LogLevel.Error);
            return true; // default to original logic
        }
    }

    #endregion harmony patches
}