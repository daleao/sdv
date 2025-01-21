﻿namespace DaLion.Professions.Framework.Patchers.Prestige;

#region using directives

using DaLion.Shared.Harmony;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class SkillsPageCtorPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="SkillsPageCtorPatcher"/> class.</summary>
    /// <param name="harmonizer">The <see cref="Harmonizer"/> instance that manages this patcher.</param>
    /// <param name="logger">A <see cref="Logger"/> instance.</param>
    internal SkillsPageCtorPatcher(Harmonizer harmonizer, Logger logger)
        : base(harmonizer, logger)
    {
        this.Target = this.RequireConstructor<SkillsPage>(typeof(int), typeof(int), typeof(int), typeof(int));
    }

    #region harmony patches

    /// <summary>
    ///     Patch to increase the width of the skills page in the game menu to fit prestige ribbons + color yellow skill
    ///     bars to green for level >10.
    /// </summary>
    [HarmonyPostfix]
    [UsedImplicitly]
    private static void SkillsPageCtorPostfix(SkillsPage __instance)
    {
        if (ShouldEnableSkillReset)
        {
            ISkill? maxSkill = null;
            var maxLength = 0;
            foreach (var skill in Skill.List)
            {
                var length = Game1.player.GetProfessionsForSkill(skill, true).Length;
                if (maxSkill is null)
                {
                    maxSkill = skill;
                    maxLength = length;
                    continue;
                }

                if (length <= maxLength &&
                    (length != maxLength || maxSkill.HasBeenReset() || !((ISkill)skill).HasBeenReset()))
                {
                    continue;
                }

                maxSkill = skill;
                maxLength = length;
            }

            if (maxLength > 1 || maxSkill?.HasBeenReset() == true)
            {
                var highestLevel = Skill.List.Cast<ISkill>()
                    .Concat(CustomSkill.Loaded.Values)
                    .Max(skill => skill.CurrentLevel);
                __instance.width += (maxLength + (highestLevel >= 10 ? 2 : 1)) * (int)Textures.STARS_SCALE * 4;
                SkillsPageDrawPatcher.RibbonXOffset = 48 - (maxLength * 12);
                SkillsPageDrawPatcher.ShouldDrawRibbons = true;
            }
            else
            {
                SkillsPageDrawPatcher.ShouldDrawRibbons = false;
            }
        }

        if (!ShouldEnablePrestigeLevels)
        {
            return;
        }

        var sourceRect = new Rectangle(16, 0, 14, 9);
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
                        1 => Farmer.miningSkill,
                        3 => Farmer.fishingSkill,
                        _ => skillIndex,
                    };

                    if (Game1.player.GetUnmodifiedSkillLevel(skillIndex) >= 15)
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

                    if (Game1.player.GetUnmodifiedSkillLevel(skillIndex) >= 20)
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
