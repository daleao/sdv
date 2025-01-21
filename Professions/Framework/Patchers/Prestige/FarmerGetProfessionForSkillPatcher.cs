namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerGetProfessionForSkillPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerGetProfessionForSkillPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerGetProfessionForSkillPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.getProfessionForSkill));
    }

    #region harmony patches

    /// <summary>Patch to force select most recent profession for skill.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool FarmerGetProfessionForSkillPrefix(
        Farmer __instance, ref int __result, int skillType, int skillLevel)
    {
        if (!Config.Skills.EnableSkillReset || skillType == Farmer.luckSkill)
        {
            return true; // run original logic
        }

        var skill = Skill.FromValue(skillType);
        var rootIndex = __instance.GetCurrentRootProfessionForSkill(skill);
        if (rootIndex == -1)
        {
            __result = -1;
            return false; // don't run original logic
        }

        var root = Profession.FromValue(rootIndex);
        __result = skillLevel switch
        {
            5 => rootIndex,
            10 => __instance.GetCurrentBranchingProfessionForRoot(root),
            _ => -1,
        };

        return false; // don't run original logic
    }

    #endregion harmony patches
}
