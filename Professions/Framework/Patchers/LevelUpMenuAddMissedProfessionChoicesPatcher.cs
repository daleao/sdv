namespace DaLion.Professions.Framework.Patchers;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[Debug]
internal sealed class LevelUpMenuAddMissedProfessionChoicesPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="LevelUpMenuAddMissedProfessionChoicesPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal LevelUpMenuAddMissedProfessionChoicesPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<LevelUpMenu>(nameof(LevelUpMenu.AddMissedProfessionChoices));
    }

    #region harmony patches

    /// <summary>Debug issues with repeated level ups.</summary>
    [HarmonyPrefix]
    [UsedImplicitly]
    private static bool LevelUpMenuAddProfessionDescriptionsPrefix(Farmer farmer)
    {
        var skills = new[] { 0, 1, 2, 3, 4 };
        foreach (var skill in skills)
        {
            if (farmer.GetUnmodifiedSkillLevel(skill) >= 5 && !farmer.newLevels.Contains(new Point(skill, 5)) &&
                farmer.getProfessionForSkill(skill, 5) == -1)
            {
                farmer.newLevels.Add(new Point(skill, 5));
            }

            if (farmer.GetUnmodifiedSkillLevel(skill) >= 10 && !farmer.newLevels.Contains(new Point(skill, 10)) &&
                farmer.getProfessionForSkill(skill, 10) == -1)
            {
                farmer.newLevels.Add(new Point(skill, 10));
            }
        }

        return false;
    }

    #endregion harmony patches
}
