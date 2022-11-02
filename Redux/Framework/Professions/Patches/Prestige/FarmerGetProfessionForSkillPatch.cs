namespace DaLion.Redux.Framework.Professions.Patches.Prestige;

#region using directives

using DaLion.Redux.Framework.Professions.Extensions;
using HarmonyLib;
using HarmonyPatch = DaLion.Shared.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetProfessionForSkillPatch : HarmonyPatch
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetProfessionForSkillPatch"/> class.</summary>
    internal FarmerGetProfessionForSkillPatch()
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.getProfessionForSkill));
    }

    #region harmony patches

    /// <summary>Patch to force select most recent profession for skill.</summary>
    [HarmonyPrefix]
    private static bool FarmerGetProfessionForSkillPrefix(
        Farmer __instance, ref int __result, int skillType, int skillLevel)
    {
        if (!ModEntry.Config.Professions.EnablePrestige || skillType == Farmer.luckSkill ||
            !Skill.TryFromValue(skillType, out var skill))
        {
            return true; // run original logic
        }

        var branch = __instance.GetCurrentBranchForSkill(skill);
        if (branch < 0)
        {
            __result = -1;
            return false; // don't run original logic
        }

        __result = skillLevel switch
        {
            5 => branch,
            10 => __instance.GetCurrentProfessionForBranch(Profession.FromValue(branch)),
            _ => -1,
        };

        return false; // don't run original logic
    }

    #endregion harmony patches
}
