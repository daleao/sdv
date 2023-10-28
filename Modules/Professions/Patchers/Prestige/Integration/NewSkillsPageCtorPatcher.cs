namespace DaLion.Overhaul.Modules.Professions.Patchers.Prestige.Integration;

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
    private static void NewSkillsPageCtorPostfix(
        NewSkillsPage __instance,
        ClickableTextureComponent ___upButton,
        ClickableTextureComponent ___downButton,
        ClickableTextureComponent ___scrollBar,
        ref Rectangle ___scrollBarRunner)
    {
        if (!ProfessionsModule.Config.EnablePrestige)
        {
            return;
        }

        __instance.width += 24;
        ___upButton.bounds.X += 24;
        ___downButton.bounds.X += 24;
        ___scrollBar.bounds.X += 24;
        ___scrollBarRunner.X += 24;
        if (ProfessionsModule.Config.PrestigeProgressionStyle == ProfessionConfig.ProgressionStyle.StackedStars)
        {
            __instance.width += 24;
            __instance.width += 24;
            ___upButton.bounds.X += 24;
            ___downButton.bounds.X += 24;
            ___scrollBar.bounds.X += 24;
            ___scrollBarRunner.X += 24;
        }

        var sourceRect = new Rectangle(16, 0, 14, 9);
        var skills = Skills.GetSkillList();
        for (var i = 0; i < __instance.skillBars.Count; i++)
        {
            var component = __instance.skillBars[i];
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
                        component.sourceRect = sourceRect;
                    }

                    break;
            }
        }
    }

    #endregion harmony patches
}
