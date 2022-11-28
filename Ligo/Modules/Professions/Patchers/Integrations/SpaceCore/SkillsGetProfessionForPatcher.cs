namespace DaLion.Ligo.Modules.Professions.Patchers.Integrations;

#region using directives

using DaLion.Ligo.Modules.Professions.Extensions;
using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class SkillsGetProfessionForPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsGetProfessionForPatcher"/> class.</summary>
    internal SkillsGetProfessionForPatcher()
    {
        this.Target = this.RequireMethod<SpaceCore.Skills>("GetProfessionFor");
    }

    #region harmony patches

    /// <summary>Patch to apply prestige ordering to skills page profession choices.</summary>
    [HarmonyPrefix]
    private static bool SkillsGetProfessionForPrefix(ref SpaceCore.Skills.Skill.Profession? __result, SpaceCore.Skills.Skill skill, int level)
    {
        if (!ModEntry.Config.Professions.EnablePrestige)
        {
            return true; // run original logic
        }

        var scSkill = SCSkill.FromSpaceCore(skill);
        if (scSkill is null)
        {
            Log.W($"The SpaceCore skill {skill.Id} was somehow not properly loaded.");
            return true; // run original logic
        }

        var tierOneIndex = Game1.player.GetCurrentBranchForSkill(scSkill);
        if (tierOneIndex == -1)
        {
            __result = null;
            return false; // don't run original logic
        }

        if (!SCProfession.Loaded.TryGetValue(tierOneIndex, out var tierOneProfession) &&
            !SCProfession.Loaded.TryGetValue(tierOneIndex - 100, out tierOneProfession))
        {
            Log.W($"The profession {tierOneIndex} was not found within the loaded SpaceCore professions.");
            return true; // run original logic
        }

        if (level == 5)
        {
            __result = tierOneProfession.ToSpaceCore();
            return false; // don't run original logic
        }

        if (level == 10)
        {
            var tierTwoIndex = Game1.player.GetCurrentProfessionForBranch(tierOneProfession);
            if (tierTwoIndex == -1)
            {
                __result = null;
                return false; // don't run original logic
            }

            if (!SCProfession.Loaded.TryGetValue(tierTwoIndex, out var tierTwoProfession) &&
                !SCProfession.Loaded.TryGetValue(tierTwoIndex - 100, out tierTwoProfession))
            {
                Log.W($"The profession {tierTwoIndex} was not found within the loaded SpaceCore professions.");
                return true; // run original logic
            }

            __result = tierTwoProfession.ToSpaceCore();
            return false; // don't run original logic
        }

        __result = null;
        return false; // don't run original logic
    }

    #endregion harmony patches
}
