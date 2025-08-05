namespace DaLion.Professions.Framework.Patchers.Prestige.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewSkillsPagePerformHoverActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewSkillsPagePerformHoverActionPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal NewSkillsPagePerformHoverActionPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireMethod<NewSkillsPage>(nameof(NewSkillsPage.performHoverAction));
    }

    #region harmony patches

    /// <summary>Patch to add prestige ribbon hover text + truncate profession descriptions in hover menu.</summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void NewSkillsPagePerformHoverActionPostfix(int x, int y, ref string ___hoverText)
    {
        ___hoverText = Game1.parseText(___hoverText, Game1.smallFont, 500);
        if (!ShouldEnableSkillReset || !NewSkillsPageDrawPatcher.ShouldDrawRibbons)
        {
            return;
        }

        foreach (var (skill, bounds) in NewSkillsPageDrawPatcher.RibbonTargetRectBySkill)
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
