namespace DaLion.Stardew.Professions.Framework.Patches.Integrations.SpaceCore;

#region using directives

using DaLion.Common.Attributes;
using DaLion.Common.Extensions.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Collections.Generic;
using Textures;

#endregion using directives

[UsedImplicitly, RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewSkillsPageCtorPatch : DaLion.Common.Harmony.HarmonyPatch
{
    /// <summary>Construct an instance.</summary>
    internal NewSkillsPageCtorPatch()
    {
        Target = "SpaceCore.Interface.NewSkillsPage".ToType()
            .RequireConstructor(new[] { typeof(int), typeof(int), typeof(int), typeof(int) });
    }

    #region harmony patches

    /// <summary>
    ///     Patch to increase the width of the skills page in the game menu to fit prestige ribbons + color yellow skill
    ///     bars to green for level >10.
    /// </summary>
    [HarmonyPostfix]
    private static void SkillsPageCtorPostfix(IClickableMenu __instance)
    {
        if (!ModEntry.Config.EnablePrestige) return;

        __instance.width += 48;
        if (ModEntry.Config.PrestigeProgressionStyle == ModConfig.ProgressionStyle.StackedStars) __instance.width += 24;

        if (__instance.GetType().RequireField("skillBars")!.GetValue(__instance) is not List<ClickableTextureComponent>
            skillBars) return;

        var srcRect = new Rectangle(16, 0, 14, 9);
        foreach (var component in skillBars)
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
                        component.texture = Textures.BarsTx;
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
                        component.texture = Textures.BarsTx;
                        component.sourceRect = srcRect;
                    }

                    break;
            }
        }
    }

    #endregion harmony patches
}