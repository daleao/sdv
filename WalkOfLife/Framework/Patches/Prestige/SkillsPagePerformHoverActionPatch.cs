using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;
using TheLion.Stardew.Common.Extensions;
using TheLion.Stardew.Professions.Framework.Extensions;

namespace TheLion.Stardew.Professions.Framework.Patches;

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

        var w = Utility.Prestige.RibbonWidth;
        var s = Utility.Prestige.RibbonScale;
        var bounds =
            new Rectangle(
                __instance.xPositionOnScreen + __instance.width + Utility.Prestige.RibbonHorizontalOffset,
                __instance.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth -
                70, (int) (w * s), (int) (w * s));

        for (var i = 0; i < 5; ++i)
        {
            bounds.Y += 56;
            if (!bounds.Contains(x, y)) continue;

            // need to do this bullshit switch because mining and fishing are inverted in the skills page
            var skillIndex = i switch
            {
                1 => 3,
                3 => 1,
                _ => i
            };

            var professionsForThisSkill = Game1.player.GetAllProfessionsForSkill(skillIndex, true).ToList();
            var count = professionsForThisSkill.Count;
            if (count == 0) continue;

            ___hoverText = ModEntry.ModHelper.Translation.Get("prestige.skillpage.tooltip", new {count});
            ___hoverText = professionsForThisSkill
                .Select(p => ModEntry.ModHelper.Translation.Get(Utility.Professions.NameOf(p).ToLower() + ".name." +
                                                                (Game1.player.IsMale ? "male" : "female")))
                .Aggregate(___hoverText, (current, name) => current + $"\n• {name}");
        }
    }

    #endregion harmony patches
}