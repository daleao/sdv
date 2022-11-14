namespace DaLion.Ligo.Modules.Professions.Patches.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using HarmonyLib;
using Shared.Harmony;

#endregion using directives

[UsedImplicitly]
[Integration("Annosz.UiInfoSuite2", "2.2.6")]
internal sealed class ExperienceBarGetExperienceRequiredToLevelPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ExperienceBarGetExperienceRequiredToLevelPatcher"/> class.</summary>
    internal ExperienceBarGetExperienceRequiredToLevelPatcher()
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
            : Constants.ExpAtLevel10 + ((currentLevel - 10 + 1) * (int)ModEntry.Config.Professions.RequiredExpPerExtendedLevel);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
