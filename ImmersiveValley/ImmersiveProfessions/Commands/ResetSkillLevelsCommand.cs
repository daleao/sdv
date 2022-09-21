namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System;
using System.Linq;
using DaLion.Common;
using DaLion.Common.Commands;
using DaLion.Common.Integrations.SpaceCore;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class ResetSkillLevelsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="ResetSkillLevelsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ResetSkillLevelsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "reset_levels", "reset_skills" };

    /// <inheritdoc />
    public override string Documentation =>
        "Reset the level of the specified skills, or all skills if none are specified. Does not remove professions.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length <= 0)
        {
            Game1.player.farmingLevel.Value = 0;
            Game1.player.fishingLevel.Value = 0;
            Game1.player.foragingLevel.Value = 0;
            Game1.player.miningLevel.Value = 0;
            Game1.player.combatLevel.Value = 0;
            Game1.player.luckLevel.Value = 0;
            Game1.player.newLevels.Clear();
            for (var i = 0; i <= 5; ++i)
            {
                Game1.player.experiencePoints[i] = 0;
                if (ModEntry.Config.ForgetRecipes && i < 5)
                {
                    Game1.player.ForgetRecipesForSkill(Skill.FromValue(i));
                }
            }

            LevelUpMenu.RevalidateHealth(Game1.player);

            foreach (var (_, skill) in CustomSkill.Loaded)
            {
                ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(Game1.player, skill.StringId, -skill.CurrentExp);
                if (ModEntry.Config.ForgetRecipes &&
                    skill.StringId == "blueberry.LoveOfCooking.CookingSkill")
                {
                    Game1.player.ForgetRecipesForLoveOfCookingSkill();
                }
            }
        }
        else
        {
            foreach (var arg in args)
            {
                if (Skill.TryFromName(arg, true, out var skill))
                {
                    skill
                        .When(Skill.Farming).Then(() => Game1.player.farmingLevel.Value = 0)
                        .When(Skill.Fishing).Then(() => Game1.player.fishingLevel.Value = 0)
                        .When(Skill.Foraging).Then(() => Game1.player.foragingLevel.Value = 0)
                        .When(Skill.Mining).Then(() => Game1.player.miningLevel.Value = 0)
                        .When(Skill.Combat).Then(() => Game1.player.combatLevel.Value = 0)
                        .When(Skill.Luck).Then(() => Game1.player.luckLevel.Value = 0);

                    Game1.player.experiencePoints[skill] = 0;
                    Game1.player.newLevels.Set(Game1.player.newLevels.Where(p => p.X != skill).ToList());
                    if (ModEntry.Config.ForgetRecipes && skill < Skill.Luck)
                    {
                        Game1.player.ForgetRecipesForSkill(skill);
                    }
                }
                else
                {
                    var customSkill = CustomSkill.Loaded.Values.FirstOrDefault(s =>
                        string.Equals(s.DisplayName, arg, StringComparison.CurrentCultureIgnoreCase));
                    if (customSkill is null)
                    {
                        Log.W($"Ignoring unknown skill {arg}.");
                        continue;
                    }

                    ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(
                        Game1.player,
                        customSkill.StringId,
                        -customSkill.CurrentExp);

                    var newLevels = ExtendedSpaceCoreApi.GetCustomSkillNewLevels.Value();
                    ExtendedSpaceCoreApi.SetCustomSkillNewLevels.Value(newLevels
                        .Where(pair => pair.Key != customSkill.StringId).ToList());

                    if (ModEntry.Config.ForgetRecipes &&
                        customSkill.StringId == "blueberry.LoveOfCooking.CookingSkill")
                    {
                        Game1.player.ForgetRecipesForLoveOfCookingSkill();
                    }
                }
            }
        }
    }
}
