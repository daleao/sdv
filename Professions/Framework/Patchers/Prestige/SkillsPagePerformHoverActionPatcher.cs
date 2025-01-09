﻿namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPagePerformHoverActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsPagePerformHoverActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal SkillsPagePerformHoverActionPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireMethod<SkillsPage>(nameof(SkillsPage.performHoverAction));
    }

    #region harmony patches

    /// <summary>Patch to add prestige ribbon hover text + truncate profession descriptions in hover menu.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void SkillsPagePerformHoverActionPostfix(int x, int y, ref string ___hoverText)
    {
        ___hoverText = Game1.parseText(___hoverText, Game1.smallFont, 500);
        if (!ShouldEnableSkillReset || !SkillsPageDrawPatcher.ShouldDrawRibbons)
        {
            return;
        }

        foreach (var (skill, bounds) in SkillsPageDrawPatcher.RibbonTargetRectBySkill)
        {
            if (!bounds.Contains(x, y))
            {
                continue;
            }

            var professionsForThisSkill = Game1.player.GetProfessionsForSkill(skill, false);
            Array.Sort(professionsForThisSkill);
            var count = professionsForThisSkill.Length;
            ___hoverText = professionsForThisSkill.Aggregate(
                I18n.Prestige_SkillPage_Tooltip(count),
                (current, p) => current + $"\n{(p.Level == 10 ? "  -" : "•")} {p.Title}");
        }
    }

    #endregion harmony patches
}
