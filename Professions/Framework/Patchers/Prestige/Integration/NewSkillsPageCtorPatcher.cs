﻿namespace DaLion.Professions.Framework.Patchers.Prestige.Integration;

#region using directives

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
internal sealed class NewSkillsPageCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="NewSkillsPageCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal NewSkillsPageCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<NewSkillsPage>(typeof(int), typeof(int), typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to increase the width of the skills page in the game menu to fit prestige ribbons + color yellow skill
    ///     bars to green for level >10.
    /// </summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void NewSkillsPageCtorPostfix(
        NewSkillsPage __instance,
        ClickableTextureComponent ___upButton,
        ClickableTextureComponent ___downButton,
        ClickableTextureComponent ___scrollBar,
        ref Rectangle ___scrollBarRunner)
    {
        if (ShouldEnableSkillReset)
        {
            var resetData = Data.Read(Game1.player, DataKeys.ResetCountBySkill).ParseDictionary<string, int>();
            if (resetData.Any())
            {
                var maxResets = resetData.Max(pair => pair.Value);
                if (maxResets > 0)
                {
                    var highestLevel = Skill.List.Cast<ISkill>()
                        .Concat(CustomSkill.Loaded.Values)
                        .Max(skill => skill.CurrentLevel);
                    var addedWidth = (maxResets + (highestLevel >= 10 ? 2 : 1)) * (int)Textures.STARS_SCALE * 4;
                    __instance.width += addedWidth;
                    ___upButton.bounds.X += addedWidth;
                    ___downButton.bounds.X += addedWidth;
                    ___scrollBar.bounds.X += addedWidth;
                    ___scrollBarRunner.X += addedWidth;
                    NewSkillsPageDrawPatcher.RibbonXOffset = 48 - (maxResets * 12);
                    NewSkillsPageDrawPatcher.ShouldDrawRibbons = true;
                }
                else
                {
                    NewSkillsPageDrawPatcher.ShouldDrawRibbons = false;
                }
            }
        }

        if (!ShouldEnablePrestigeLevels)
        {
            return;
        }

        var sourceRect = new Rectangle(16, 0, 14, 9);
        var skills = SCSkills.GetSkillList();
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
                        1 => Farmer.miningSkill,
                        3 => Farmer.fishingSkill,
                        _ => skillIndex,
                    };

                    skillLevel = skillIndex switch
                    {
                        < Farmer.luckSkill => Game1.player.GetUnmodifiedSkillLevel(skillIndex),
                        > Farmer.luckSkill => SCSkills.GetSkillLevel(Game1.player, skills[skillIndex - Farmer.luckSkill - 1]), // -5 for vanilla skills and -1 for zero-index
                        _ => 0,
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
                        1 => Farmer.miningSkill,
                        3 => Farmer.fishingSkill,
                        _ => skillIndex,
                    };

                    skillLevel = skillIndex switch
                    {
                        < 5 => Game1.player.GetUnmodifiedSkillLevel(skillIndex),
                        > Farmer.luckSkill => SCSkills.GetSkillLevel(Game1.player, skills[skillIndex - Farmer.luckSkill - 1]), // -5 for vanilla skills and -1 for zero-index
                        _ => 0,
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
