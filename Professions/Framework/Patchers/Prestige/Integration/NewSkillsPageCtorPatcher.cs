namespace DaLion.Professions.Framework.Patchers.Prestige.Integration;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore;
using SpaceCore.Interface;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
[ModRequirement("spacechase0.SpaceCore")]
internal sealed class NewSkillsPageCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewSkillsPageCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    internal NewSkillsPageCtorPatcher(Harmonizer harmonizer)
        : base(harmonizer)
    {
        this.Target = this.RequireConstructor<NewSkillsPage>(typeof(int), typeof(int), typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to increase the width of the skills page in the game menu to fit prestige ribbons + color yellow skill
    ///     bars to green for level >10.
    /// </summary>
    [HarmonyPostfix]
    private static void NewSkillsPageCtorPostfix(
        NewSkillsPage __instance,
        ClickableTextureComponent ___upButton,
        ClickableTextureComponent ___downButton,
        ClickableTextureComponent ___scrollBar,
        ref Rectangle ___scrollBarRunner)
    {
        if (EnableSkillReset)
        {
            var max = Skill.List.Select(skill => Game1.player.GetProfessionsForSkill(skill, true).Length).Max();
            if (max > 0)
            {
                var width = (max + 2) * 4 * (int)Textures.STARS_SCALE;
                __instance.width += width;
                ___upButton.bounds.X += width;
                ___downButton.bounds.X += width;
                ___scrollBar.bounds.X += width;
                ___scrollBarRunner.X += width;
            }
        }

        if (!EnablePrestigeLevels)
        {
            return;
        }

        var sourceRect = new Rectangle(16, 0, 14, 9);
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
                        _ => SCSkills.GetSkillLevel(Game1.player, skills[skillIndex - 5]),
                    };

                    if (skillLevel >= 15)
                    {
                        component.texture = Textures.SkillBars;
                        component.sourceRect = sourceRect;
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
                        _ => SCSkills.GetSkillLevel(Game1.player, skills[skillIndex - 5]),
                    };

                    if (skillLevel >= 20)
                    {
                        component.texture = Textures.SkillBars;
                        component.sourceRect = sourceRect;
                    }

                    break;
            }
        }
    }

    #endregion harmony patches
}
