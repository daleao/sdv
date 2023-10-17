namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige.Integration;

#region using directives

using System.Linq;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore.Interface;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewSkillsPagePerformHoverActionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewSkillsPagePerformHoverActionPatcher"/> class.</summary>
    internal NewSkillsPagePerformHoverActionPatcher()
    {
        this.Target = this.RequireMethod<NewSkillsPage>(nameof(NewSkillsPage.performHoverAction));
    }

    #region harmony patches

    /// <summary>Patch to add prestige ribbon hover text + truncate profession descriptions in hover menu.</summary>
    [HarmonyPostfix]
    private static void NewSkillsPagePerformHoverActionPostfix(
        NewSkillsPage __instance, int x, int y, ref string ___hoverText, int ___skillScrollOffset)
    {
        ___hoverText = ___hoverText.Truncate(90);

        if (!ProfessionsModule.Config.EnablePrestige)
        {
            return;
        }

        var bounds = ProfessionsModule.Config.ProgressionStyle switch
        {
            Config.PrestigeProgressionStyle.StackedStars => new Rectangle(
                __instance.xPositionOnScreen + __instance.width + Textures.ProgressionHorizontalOffset - 22,
                __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Textures.ProgressionVerticalOffset + 8,
                0,
                (int)(Textures.SingleStarWidth * Textures.StarsScale)),
            Config.PrestigeProgressionStyle.Gen3Ribbons or Config.PrestigeProgressionStyle.Gen4Ribbons => new Rectangle(
                __instance.xPositionOnScreen + __instance.width + Textures.ProgressionHorizontalOffset,
                __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth + Textures.ProgressionVerticalOffset,
                (int)(Textures.RibbonWidth * Textures.RibbonScale),
                (int)(Textures.RibbonWidth * Textures.RibbonScale)),
            _ => Rectangle.Empty,
        };

        var lastVisibleSkillIndex =
            Reflector.GetUnboundPropertyGetter<NewSkillsPage, int>("LastVisibleSkillIndex").Invoke(__instance);
        for (var i = 0; i < 5; i++)
        {
            // hide if scrolled and out of bounds
            if (i < ___skillScrollOffset || i > lastVisibleSkillIndex)
            {
                continue;
            }

            bounds.Y += 56;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i),
            };

            var professionsForThisSkill = Game1.player.GetProfessionsForSkill(skill, true);
            var count = professionsForThisSkill.Length;
            if (count == 0)
            {
                continue;
            }

            bounds.Width = ProfessionsModule.Config.ProgressionStyle is Config.PrestigeProgressionStyle.Gen3Ribbons
                or Config.PrestigeProgressionStyle.Gen4Ribbons
                ? (int)(Textures.RibbonWidth * Textures.RibbonScale)
                : (int)(((Textures.SingleStarWidth / 2 * count) + 4) * Textures.StarsScale);
            if (!bounds.Contains(x, y))
            {
                continue;
            }

            ___hoverText = I18n.Prestige_SkillPage_Tooltip(count);
            for (var j = 0; j < professionsForThisSkill.Length; j++)
            {
                ___hoverText += $"\n• {professionsForThisSkill[j].Title}";
            }
        }

        if (SCSkill.Loaded.Count == 0)
        {
            return;
        }

        var customSkills = SpaceCoreIntegration.Instance!.ModApi!
            .GetCustomSkills()
            .Select(name => SCSkill.Loaded[name]);
        if (LuckSkill.Instance is not null)
        {
            // luck skill must be enumerated first
            customSkills = customSkills.Prepend(LuckSkill.Instance);
        }

        var indexWithLuckSkill =
            Reflector.GetUnboundPropertyGetter<NewSkillsPage, int>("GameSkillCount").Invoke(__instance) - 1;
        foreach (var skill in customSkills)
        {
            // hide if scrolled and out of bounds
            if (indexWithLuckSkill < ___skillScrollOffset || indexWithLuckSkill > lastVisibleSkillIndex)
            {
                indexWithLuckSkill++;
                continue;
            }

            bounds.Y += 56;

            var professionsForThisSkill =
                Game1.player.GetProfessionsForSkill(skill, true);
            var count = professionsForThisSkill.Length;
            if (count == 0)
            {
                indexWithLuckSkill++;
                continue;
            }

            bounds.Width = ProfessionsModule.Config.ProgressionStyle is Config.PrestigeProgressionStyle.Gen3Ribbons
                or Config.PrestigeProgressionStyle.Gen4Ribbons
                ? (int)(Textures.RibbonWidth * Textures.RibbonScale)
                : (int)(((Textures.SingleStarWidth / 2 * count) + 4) * Textures.StarsScale);
            if (!bounds.Contains(x, y))
            {
                indexWithLuckSkill++;
                continue;
            }

            ___hoverText = I18n.Prestige_SkillPage_Tooltip(count);
            for (var j = 0; j < professionsForThisSkill.Length; j++)
            {
                ___hoverText += $"\n• {professionsForThisSkill[j].Title}";
            }

            indexWithLuckSkill++;
        }
    }

    #endregion harmony patches
}
