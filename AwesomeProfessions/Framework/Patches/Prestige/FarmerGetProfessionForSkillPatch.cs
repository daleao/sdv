using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;
using DaLion.Stardew.Professions.Framework.Extensions;

namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

[UsedImplicitly]
internal class FarmerGetProfessionForSkill : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerGetProfessionForSkill()
    {
        Original = RequireMethod<Farmer>(nameof(Farmer.getProfessionForSkill));
    }

    #region harmony patches

    /// <summary>Patch to force select most recent profession for skill.</summary>
    [HarmonyPrefix]
    private static bool FarmerGetProfessionForSkillPrefix(Farmer __instance, ref int __result, int skillType,
        int skillLevel)
    {
        if (!ModEntry.Config.EnablePrestige) return true; // run original logic

        var branch = __instance.GetCurrentBranchForSkill(skillType);
        __result = skillLevel switch
        {
            5 => branch,
            10 => __instance.GetCurrentProfessionForBranch(branch),
            _ => -1
        };

        return false; // don't run original logic
    }

    #endregion harmony patches
}