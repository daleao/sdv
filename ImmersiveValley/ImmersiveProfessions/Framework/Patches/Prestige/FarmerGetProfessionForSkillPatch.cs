namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;

#region using directives

using HarmonyLib;
using JetBrains.Annotations;
using StardewValley;

using Extensions;

#endregion using directives

[UsedImplicitly]
internal class FarmerGetProfessionForSkillPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal FarmerGetProfessionForSkillPatch()
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
        if (branch < 0)
        {
            __result = -1;
            return false; // don't run original logic
        }

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