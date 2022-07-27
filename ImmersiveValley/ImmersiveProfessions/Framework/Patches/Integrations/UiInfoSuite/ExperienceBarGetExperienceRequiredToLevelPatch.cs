namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.UiInfoSuite;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;

#endregion using directives

[UsedImplicitly, RequiresMod("Annosz.UiInfoSuite2")]
internal sealed class ExperienceBarGetExperienceRequiredToLevelPatch : DaLion.Common.Harmony.HarmonyPatch
{
    private const int EXP_AT_LEVEL_TEN_I = 15000;

    /// <summary>Construct an instance.</summary>
    internal ExperienceBarGetExperienceRequiredToLevelPatch()
    {
        Target = "UIInfoSuite.UIElements.ExperienceBar".ToType().RequireMethod("GetExperienceRequiredToLevel");
    }

    #region harmony patches

    /// <summary>Patch to reflect adjusted base experience + extended progression experience.</summary>
    [HarmonyPrefix]
    private static bool ExperienceBarGetExperienceRequiredToLevelPrefix(ref int __result, int currentLevel)
    {
        if (currentLevel < 10) return true; // run original logic

        __result = currentLevel >= 20
            ? 0
            : EXP_AT_LEVEL_TEN_I + (currentLevel - 10 + 1) * (int)ModEntry.Config.RequiredExpPerExtendedLevel;
        return false; // don't run original logic
    }

    #endregion harmony patches
}