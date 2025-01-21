namespace DaLion.Professions.Framework.Patchers.Prestige.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class SkillsGetProfessionForPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsGetProfessionForPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SkillsGetProfessionForPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<SCSkills>("GetProfessionFor");
    }

    #region harmony patches

    /// <summary>Patch to apply prestige ordering to skills page profession choices.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool SkillsGetProfessionForPrefix(ref SCProfession? __result, SCSkill skill, int level)
    {
        if (!ShouldEnableSkillReset)
        {
            return true; // run original logic
        }

        var customSkill = CustomSkill.GetFromSpaceCore(skill);
        if (customSkill is null)
        {
            Log.W($"The SpaceCore skill {skill.Id} is loaded, but failed to be mapped.");
            return true; // run original logic
        }

        var root = Game1.player.GetCurrentRootProfessionForSkill(customSkill);
        if (root == -1)
        {
            __result = null;
            return false; // don't run original logic
        }

        if (!CustomProfession.Loaded.TryGetValue(root, out var tierOneProfession) &&
            !CustomProfession.Loaded.TryGetValue(root - 100, out tierOneProfession))
        {
            Log.W($"The profession {root} was not found within the loaded SpaceCore professions.");
            return true; // run original logic
        }

        switch (level)
        {
            case 5:
                __result = tierOneProfession.ToSpaceCore();
                return false; // don't run original logic
            case 10:
            {
                var branch = Game1.player.GetCurrentBranchingProfessionForRoot(tierOneProfession);
                if (branch == -1)
                {
                    __result = null;
                    return false; // don't run original logic
                }

                if (!CustomProfession.Loaded.TryGetValue(branch, out var tierTwoProfession) &&
                    !CustomProfession.Loaded.TryGetValue(branch - 100, out tierTwoProfession))
                {
                    Log.W($"The profession {branch} was not found within the loaded SpaceCore professions.");
                    return true; // run original logic
                }

                __result = tierTwoProfession.ToSpaceCore();
                return false; // don't run original logic
            }

            default:
                __result = null;
                return false; // don't run original logic
        }
    }

    #endregion harmony patches
}
