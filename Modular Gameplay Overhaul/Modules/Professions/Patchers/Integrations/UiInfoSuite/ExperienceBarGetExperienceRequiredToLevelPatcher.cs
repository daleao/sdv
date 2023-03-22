namespace DaLion.Overhaul.Modules.Professions.Patchers.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
[RequiresMod("Annosz.UiInfoSuite2", version: "2.2.6")]
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
            : ISkill.ExpAtLevel10 + ((currentLevel - 10 + 1) * (int)ProfessionsModule.Config.RequiredExpPerExtendedLevel);
        return false; // don't run original logic
    }

    #endregion harmony patches
}
