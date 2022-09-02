namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions;
using DaLion.Common.Extensions.Reflection;
using Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Linq;
using Textures;

#endregion using directives

[UsedImplicitly, RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewSkillsPagePerformHoverActionPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal NewSkillsPagePerformHoverActionPatch()
    {
        Target = "SpaceCore.Interface.NewSkillsPage".ToType().RequireMethod("performHoverAction");
    }

    #region harmony patches

    /// <summary>Patch to add prestige ribbon hover text + truncate profession descriptions in hover menu.</summary>
    [HarmonyPostfix]
    private static void NewSkillsPagePerformHoverActionPostfix(IClickableMenu __instance, int x, int y,
        ref string ___hoverText)
    {
        ___hoverText = ___hoverText.Truncate(90);

        if (!ModEntry.Config.EnablePrestige) return;

        Rectangle bounds;
        switch (ModEntry.Config.PrestigeProgressionStyle)
        {
            case ModConfig.ProgressionStyle.StackedStars:
                bounds = new(
                    __instance.xPositionOnScreen + __instance.width + Textures.PROGRESSION_HORIZONTAL_OFFSET_I - 22,
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth +
                    Textures.PROGRESSION_VERTICAL_OFFSET_I + 8, 0,
                    (int)(Textures.SINGLE_STAR_WIDTH_I * Textures.STARS_SCALE_F)
                );
                break;
            case ModConfig.ProgressionStyle.Gen3Ribbons:
            case ModConfig.ProgressionStyle.Gen4Ribbons:
                bounds = new(
                    __instance.xPositionOnScreen + __instance.width + Textures.PROGRESSION_HORIZONTAL_OFFSET_I,
                    __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth +
                    Textures.PROGRESSION_VERTICAL_OFFSET_I, (int)(Textures.RIBBON_WIDTH_I * Textures.RIBBON_SCALE_F),
                    (int)(Textures.RIBBON_WIDTH_I * Textures.RIBBON_SCALE_F));
                break;
            default:
                bounds = Rectangle.Empty;
                break;
        }

        for (var i = 0; i < 5; ++i)
        {
            bounds.Y += 56;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skill = i switch
            {
                1 => Skill.Mining,
                3 => Skill.Fishing,
                _ => Skill.FromValue(i)
            };
            var professionsForThisSkill = Game1.player.GetProfessionsForSkill(skill, true).ToArray();
            var count = professionsForThisSkill.Length;
            if (count == 0) continue;

            bounds.Width = ModEntry.Config.PrestigeProgressionStyle is ModConfig.ProgressionStyle.Gen3Ribbons
                or ModConfig.ProgressionStyle.Gen4Ribbons
                ? (int)(Textures.RIBBON_WIDTH_I * Textures.RIBBON_SCALE_F)
                : (int)((Textures.SINGLE_STAR_WIDTH_I / 2 * count + 4) * Textures.STARS_SCALE_F);
            if (!bounds.Contains(x, y)) continue;

            ___hoverText = ModEntry.i18n.Get("prestige.skillpage.tooltip", new { count });
            ___hoverText = professionsForThisSkill
                .Select(p => p.GetDisplayName(Game1.player.IsMale))
                .Aggregate(___hoverText, (current, name) => current + $"\n• {name}");
        }

        if (ModEntry.SpaceCoreApi is null) return;

        foreach (var skill in CustomSkill.LoadedSkills.Values)
        {
            bounds.Y += 56;
            var professionsForThisSkill =
                Game1.player.GetProfessionsForSkill(skill, true).ToArray();
            var count = professionsForThisSkill.Length;
            if (count == 0) continue;

            bounds.Width = ModEntry.Config.PrestigeProgressionStyle is ModConfig.ProgressionStyle.Gen3Ribbons
                or ModConfig.ProgressionStyle.Gen4Ribbons
                ? (int)(Textures.RIBBON_WIDTH_I * Textures.RIBBON_SCALE_F)
                : (int)((Textures.SINGLE_STAR_WIDTH_I / 2 * count + 4) * Textures.STARS_SCALE_F);
            if (!bounds.Contains(x, y)) continue;

            ___hoverText = ModEntry.i18n.Get("prestige.skillpage.tooltip", new { count });
            ___hoverText = professionsForThisSkill
                .Select(p => p.GetDisplayName())
                .Aggregate(___hoverText, (current, name) => current + $"\n• {name}");
        }
    }

    #endregion harmony patches
}