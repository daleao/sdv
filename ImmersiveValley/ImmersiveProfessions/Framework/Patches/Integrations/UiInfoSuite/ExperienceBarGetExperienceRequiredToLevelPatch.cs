namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.UiInfoSuite;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;
using HarmonyPatch = DaLion.Common.Harmony.HarmonyPatch;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Annosz.UiInfoSuite2", "2.2.6")]
internal sealed class ExperienceBarGetExperienceRequiredToLevelPatch : HarmonyPatch
{
    private const int ExpAtLevelTen = 15000;

    /// <summary>Initializes a new instance of the <see cref="ExperienceBarGetExperienceRequiredToLevelPatch"/> class.</summary>
    internal ExperienceBarGetExperienceRequiredToLevelPatch()
    {
        this.Target = "UIInfoSuite2.UIElements.ExperienceBar"
            .ToType()
            .RequireMethod("GetExperienceRequiredToLevel");
    }

    #region harmony patches

    /// <summary>Patch to reflect adjusted base experience + extended progression experience.</summary>
    [HarmonyPrefix]
    private static bool ExperienceBarGetExperienceRequiredToLevelPrefix(ref int __result, int currentLevel)
    {
        if (currentLevel < 10)
        {
            return true; // run original logic
        }

        __result = currentLevel >= 20
            ? 0
            : ExpAtLevelTen + ((currentLevel - 10 + 1) * (int)ModEntry.Config.RequiredExpPerExtendedLevel);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
