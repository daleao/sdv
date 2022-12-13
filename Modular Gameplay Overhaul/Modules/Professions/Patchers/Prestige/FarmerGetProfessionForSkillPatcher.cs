namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetProfessionForSkillPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetProfessionForSkillPatcher"/> class.</summary>
    internal FarmerGetProfessionForSkillPatcher()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.getProfessionForSkill));
    }

    #region harmony patches

    /// <summary>Patch to force select most recent profession for skill.</summary>
    [HarmonyPrefix]
    private static bool FarmerGetProfessionForSkillPrefix(
        Farmer __instance, ref int __result, int skillType, int skillLevel)
    {
        if (!ProfessionsModule.Config.EnablePrestige || skillType == Farmer.luckSkill)
        {
            return true; // run original logic
        }

        if (!Skill.TryFromValue(skillType, out var skill))
        {
            Log.W($"Received some unknown vanilla skill type ({skillType}).");
            return true; // run original logic
        }

        var tierOneIndex = __instance.GetCurrentBranchForSkill(skill);
        if (tierOneIndex < 0)
        {
            __result = -1;
            return false; // don't run original logic
        }

        if (!Profession.TryFromValue(tierOneIndex, out var tierOneProfession) &&
            !Profession.TryFromValue(tierOneIndex - 100, out tierOneProfession))
        {
            Log.W($"Received some unknown vanilla profession ({skillType}).");
            return true; // run original logic
        }

        __result = skillLevel switch
        {
            5 => tierOneIndex,
            10 => __instance.GetCurrentProfessionForBranch(tierOneProfession),
            _ => -1,
        };

        if (__result >= 100)
        {
            __result -= 100;
        }

        return false; // don't run original logic
    }

    #endregion harmony patches
}
