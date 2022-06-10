namespace DaLion.Stardew.Professions.Framework.Patches.Prestige;
    
#region using directives

using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Menus;

using DaLion.Common.Extensions;
using Extensions;
using Utility;

#endregion using directives

[UsedImplicitly]
internal class SkillsPagePerformHoverActionPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillsPagePerformHoverActionPatch()
    {
        Original = RequireMethod<SkillsPage>(nameof(SkillsPage.performHoverAction));
    }

    #region harmony patches

    /// <summary>Patch to add prestige ribbon hover text + truncate profession descriptions in hover menu.</summary>
    [HarmonyPostfix]
    private static void SkillsPagePerformHoverActionPostfix(SkillsPage __instance, int x, int y,
        ref string ___hoverText)
    {
        ___hoverText = ___hoverText?.Truncate(90);

        if (!ModEntry.Config.EnablePrestige) return;

        Rectangle bounds;
        switch (ModEntry.Config.Progression)
        {
            case ModConfig.ProgressionStyle.StackedStars:
                bounds = new(
                    __instance.xPositionOnScreen + __instance.width + Textures.PROGRESSION_HORIZONTAL_OFFSET_I - 14,
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth +
                    Textures.PROGRESSION_VERTICAL_OFFSET_I - 4, (int) (Textures.STARS_WIDTH_I * Textures.STARS_SCALE_F),
                    (int) (Textures.STARS_WIDTH_I * Textures.STARS_SCALE_F)
                );
                break;
            case ModConfig.ProgressionStyle.Gen3Ribbons:
            case ModConfig.ProgressionStyle.Gen4Ribbons:
                bounds = new(
                    __instance.xPositionOnScreen + __instance.width + Textures.PROGRESSION_HORIZONTAL_OFFSET_I,
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth +
                    Textures.PROGRESSION_VERTICAL_OFFSET_I, (int) (Textures.RIBBON_WIDTH_I * Textures.RIBBON_SCALE_F),
                    (int) (Textures.RIBBON_WIDTH_I * Textures.RIBBON_SCALE_F));
                break;
            default:
                bounds = Rectangle.Empty;
                break;
        }

        for (var i = 0; i < 5; ++i)
        {
            bounds.Y += 56;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skillIndex = i switch
            {
                1 => 3,
                3 => 1,
                _ => i
            };
            var professionsForThisSkill = Game1.player.GetAllProfessionsForSkill(skillIndex, true).ToList();
            var numProfessions = professionsForThisSkill.Count;
            if (numProfessions == 0) continue;

            bounds.Width = ModEntry.Config.Progression.ToString().Contains("Ribbons")
                ? (int) (Textures.RIBBON_WIDTH_I * Textures.RIBBON_SCALE_F)
                : (int) ((Textures.SINGLE_STAR_WIDTH_I / 2 * numProfessions + 4) * Textures.STARS_SCALE_F);
            if (!bounds.Contains(x, y)) continue;

            ___hoverText = ModEntry.i18n.Get("prestige.skillpage.tooltip", new {count = numProfessions});
            ___hoverText = professionsForThisSkill
                .Select(p => ModEntry.i18n.Get(p.ToProfessionName().ToLowerInvariant() + ".name." +
                                               (Game1.player.IsMale ? "male" : "female")))
                .Aggregate(___hoverText, (current, name) => current + $"\n• {name}");
        }

        if (ModEntry.SpaceCoreApi is null) return;

        foreach (var skill in ModEntry.CustomSkills.Values)
        {
            bounds.Y += 56;
            var professionsForThisSkill = Game1.player.GetAllProfessionsForCustomSkill(skill, true).ToList();
            var numProfessions = professionsForThisSkill.Count;
            if (numProfessions == 0) continue;

            bounds.Width = ModEntry.Config.Progression.ToString().Contains("Ribbons")
                ? (int) (Textures.RIBBON_WIDTH_I * Textures.RIBBON_SCALE_F)
                : (int) ((Textures.SINGLE_STAR_WIDTH_I / 2 * numProfessions + 4) * Textures.STARS_SCALE_F);
            if (!bounds.Contains(x, y)) continue;

            ___hoverText = ModEntry.i18n.Get("prestige.skillpage.tooltip", new {count = numProfessions});
            ___hoverText = professionsForThisSkill
                .Select(p => skill.ProfessionNamesById[p])
                .Aggregate(___hoverText, (current, name) => current + $"\n• {name}");
        }
    }

    #endregion harmony patches
}