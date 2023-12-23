namespace DaLion.Overhaul.Modules.Professions.Patchers;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;

#endregion using directives

[UsedImplicitly]
internal sealed class GameLocationShowQiCatInnerPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="GameLocationShowQiCatInnerPatcher"/> class.</summary>
    internal GameLocationShowQiCatInnerPatcher()
    {
        this.Target = typeof(GameLocation).GetInnerMethodsContaining("<ShowQiCat>b__303_3").SingleOrDefault();
    }

    #region harmony patches

    /// <summary>Patch to display new perfection requirement.</summary>
    [HarmonyPrefix]
    // ReSharper disable once UnusedParameter.Local
    private static bool GameLocationShowQiCatInnerPrefix(ref float __result)
    {
        if (!ProfessionsModule.EnablePrestige || !ProfessionsModule.Config.Prestige.IsPerfectionRequirement)
        {
            return true; // run original logic
        }

        if (ProfessionsModule.EnablePrestigeLevels)
        {
            // ReSharper disable once RedundantAssignment
            __result = Math.Min(
                Skill.ListVanilla
                    .Where(skill => skill.CurrentLevel >= 20)
                    .Sum(_ => 1f),
                5f);

            return false; // don't run original logic
        }

        if (ProfessionsModule.EnableSkillReset)
        {
            // ReSharper disable once RedundantAssignment
            __result += Math.Min(
                Skill.ListVanilla
                    .Where(skill => Game1.player.HasAllProfessionsInSkill(skill))
                    .Sum(_ => 1f),
                5f);

            return false; // don't run original logic
        }

        return true; // run original logic
    }

    #endregion harmony patches
}
