namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class FarmerCheckForLevelGainPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="FarmerCheckForLevelGainPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal FarmerCheckForLevelGainPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<Farmer>(nameof(Farmer.checkForLevelGain));
    }

    #region harmony patches

    /// <summary>Patch to allow level increase up to 20.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void FarmerCheckForLevelGainPostfix(ref int __result, int oldXP, int newXP)
    {
        if (!ShouldEnablePrestigeLevels)
        {
            return;
        }

        for (var i = 1; i <= 10; i++)
        {
            var requiredExpForThisLevel = ISkill.LEVEL_10_EXP + (Config.Masteries.ExpPerPrestigeLevel * i);
            if (oldXP >= requiredExpForThisLevel)
            {
                continue;
            }

            if (newXP < requiredExpForThisLevel)
            {
                return;
            }

            __result = i + 10;
        }
    }

    #endregion harmony patches
}
