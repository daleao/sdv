using HarmonyLib;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace TheLion.Stardew.Professions.Framework.Patches.Prestige;

[UsedImplicitly]
internal class SkillsPageCtorPatch : BasePatch
{
    /// <summary>Construct an instance.</summary>
    internal SkillsPageCtorPatch()
    {
        Original = RequireConstructor<SkillsPage>(typeof(int), typeof(int), typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to increase the width of the skills page in the game menu to fit prestige ribbons + color yellow skill
    ///     bars to green for level >10.
    /// </summary>
    [HarmonyPostfix]
    private static void SkillsPageCtorPostfix(ref SkillsPage __instance)
    {
        if (!ModEntry.Config.EnablePrestige) return;

        __instance.width += 64;

        var srcRect = new Rectangle(16, 0, 14, 9);
        foreach (var component in __instance.skillBars)
        {
            int skillIndex;
            switch (component.myID / 100)
            {
                case 1:
                    skillIndex = component.myID % 100;

                    // need to do this bullshit switch because mining and fishing are inverted in the skills page
                    skillIndex = skillIndex switch
                    {
                        1 => 3,
                        3 => 1,
                        _ => skillIndex
                    };

                    if (Game1.player.GetUnmodifiedSkillLevel(skillIndex) >= 15)
                    {
                        component.texture = Utility.Textures.SkillBarTx;
                        component.sourceRect = srcRect;
                    }

                    break;

                case 2:
                    skillIndex = component.myID % 200;

                    // need to do this bullshit switch because mining and fishing are inverted in the skills page
                    skillIndex = skillIndex switch
                    {
                        1 => 3,
                        3 => 1,
                        _ => skillIndex
                    };

                    if (Game1.player.GetUnmodifiedSkillLevel(skillIndex) >= 20)
                    {
                        component.texture = Utility.Textures.SkillBarTx;
                        component.sourceRect = srcRect;
                    }

                    break;
            }
        }
    }

    #endregion harmony patches
}