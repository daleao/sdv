using HarmonyLib;
using JetBrains.Annotations;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches.Compatibility.UiInfoSuite;

[UsedImplicitly]
internal class ExperienceBarGetExperienceRequiredToLevelPatch : BasePatch
{
    private const int EXP_AT_LEVEL_TEN_I = 15000;

    /// <summary>Construct an instance.</summary>
    internal ExperienceBarGetExperienceRequiredToLevelPatch()
    {
        try
        {
            Original = "UIInfoSuite.UIElements.ExperienceBar".ToType().MethodNamed("GetExperienceRequiredToLevel");
        }
        catch
        {
            // ignored
        }
    }

    #region harmony patches

    /// <summary>Patch to reflect adjusted base experience + extended progression experience.</summary>
    [HarmonyPrefix]
    private static bool GetExperienceRequiredToLevelPrefix(ref int __result, int currentLevel)
    {
        if (currentLevel < 10) return true; // run original logic

        __result = currentLevel >= 20
            ? 0
            : EXP_AT_LEVEL_TEN_I + (currentLevel - 10 + 1) * (int) ModEntry.Config.RequiredExpPerExtendedLevel;
        return false; // don't run original logic
    }

    #endregion harmony patches
}