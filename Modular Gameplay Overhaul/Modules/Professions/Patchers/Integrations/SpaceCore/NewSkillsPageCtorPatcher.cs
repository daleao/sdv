namespace DaLion.Overhaul.Modules.Professions.Patchers.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore;
using SpaceCore.Interface;

#endregion using directives

[UsedImplicitly]
[RequiresMod("spacechase0.SpaceCore")]
internal sealed class NewSkillsPageCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewSkillsPageCtorPatcher"/> class.</summary>
    internal NewSkillsPageCtorPatcher()
    {
        this.Target = this.RequireConstructor<NewSkillsPage>(typeof(int), typeof(int), typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to increase the width of the skills page in the game menu to fit prestige ribbons + color yellow skill
    ///     bars to green for level >10.
    /// </summary>
    [HarmonyPostfix]
    private static void NewSkillsPageCtorPostfix(NewSkillsPage __instance)
    {
        if (!ProfessionsModule.Config.EnablePrestige)
        {
            return;
        }

        __instance.width += 48;
        if (ProfessionsModule.Config.PrestigeProgressionStyle == Config.ProgressionStyle.StackedStars)
        {
            __instance.width += 24;
        }

        var srcRect = new Rectangle(16, 0, 14, 9);
        var skills = Skills.GetSkillList();
        foreach (var component in __instance.skillBars)
        {
            int skillIndex, skillLevel;
            switch (component.myID / 100)
            {
                case 1:
                    skillIndex = component.myID % 100;

                    // need to do this bullshit switch because mining and fishing are inverted in the skills page
                    skillIndex = skillIndex switch
                    {
                        1 => 3,
                        3 => 1,
                        _ => skillIndex,
                    };

                    skillLevel = skillIndex switch
                    {
                        < 5 => Game1.player.GetUnmodifiedSkillLevel(skillIndex),
                        > 5 => Skills.GetSkillLevel(
                            Game1.player,
                            skills[skillIndex - (LuckSkill.Instance is null ? 5 : 6)]),
                        _ => LuckSkill.Instance is not null
                            ? Game1.player.GetUnmodifiedSkillLevel(skillIndex)
                            : Skills.GetSkillLevel(Game1.player, skills[skillIndex - 5]),
                    };

                    if (skillLevel >= 15)
                    {
                        component.texture = Textures.SkillBarsTx;
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
                        _ => skillIndex,
                    };

                    skillLevel = skillIndex switch
                    {
                        < 5 => Game1.player.GetUnmodifiedSkillLevel(skillIndex),
                        > 5 => Skills.GetSkillLevel(
                            Game1.player,
                            skills[skillIndex - (LuckSkill.Instance is null ? 5 : 6)]),
                        _ => LuckSkill.Instance is not null
                            ? Game1.player.GetUnmodifiedSkillLevel(skillIndex)
                            : Skills.GetSkillLevel(Game1.player, skills[skillIndex - 5]),
                    };

                    if (skillLevel >= 20)
                    {
                        component.texture = Textures.SkillBarsTx;
                        component.sourceRect = srcRect;
                    }

                    break;
            }
        }
    }

    #endregion harmony patches
}
