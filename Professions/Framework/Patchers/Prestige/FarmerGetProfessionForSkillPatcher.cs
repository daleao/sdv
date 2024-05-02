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
    internal FarmerGetProfessionForSkillPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.getProfessionForSkill));
    }

    #region harmony patches

    /// <summary>Patch to force select most recent profession for skill.</summary>
    [HarmonyPrefix]
    private static bool FarmerGetProfessionForSkillPrefix(
        Farmer __instance, ref int __result, int skillType, int skillLevel)
    {
        if (!Config.Skills.EnableSkillReset || skillType == Farmer.luckSkill)
        {
            return true; // run original logic
        }

        if (!Skill.TryFromValue(skillType, out var skill))
        {
            Log.W($"Received some unknown vanilla skill type ({skillType}).");
            return true; // run original logic
        }

        var root = __instance.GetCurrentRootProfessionForSkill(skill);
        switch (root)
        {
            case < 0:
                __result = -1;
                return false; // don't run original logic
            case >= 100:
                root -= 100;
                break;
        }

        if (!Profession.TryFromValue(root, out var tierOneProfession))
        {
            Log.W($"Received some unknown vanilla profession ({skillType}).");
            return true; // run original logic
        }

        __result = skillLevel switch
        {
            5 => root,
            10 => __instance.GetCurrentBranchingProfessionForRoot(tierOneProfession),
            _ => -1,
        };

        return false; // don't run original logic
    }

    #endregion harmony patches
}
