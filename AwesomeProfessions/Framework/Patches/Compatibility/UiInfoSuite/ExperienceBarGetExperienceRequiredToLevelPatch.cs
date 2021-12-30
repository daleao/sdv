using HarmonyLib;
using JetBrains.Annotations;
using TheLion.Stardew.Common.Harmony;

namespace TheLion.Stardew.Professions.Framework.Patches;

[UsedImplicitly]
internal class ExperienceBarGetExperienceRequiredToLevelPatch : BasePatch
{
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
        if (currentLevel <= 10) return true; // run original logic

        __result = (int) ModEntry.Config.RequiredExpPerExtendedLevel;
        return false; // don't run original logic
    }

    #endregion harmony patches
}