namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Integrations.SpaceCore;
using Extensions;
using Framework;
using StardewValley.Menus;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class ResetSkillLevelsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal ResetSkillLevelsCommand(CommandHandler handler)
        : base(handler) { }

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
                    Game1.player.ForgetRecipesForSkill(Skill.FromValue(i));
            }

            LevelUpMenu.RevalidateHealth(Game1.player);

            foreach (var (_, skill) in CustomSkill.LoadedSkills)
            {
                ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(Game1.player, skill.StringId, -skill.CurrentExp);
                if (ModEntry.Config.ForgetRecipes &&
                    skill.StringId == "blueberry.LoveOfCooking.CookingSkill")
                    Game1.player.ForgetRecipesForLoveOfCookingSkill();
            }
        }
        else
        {
            foreach (var arg in args)
                if (Skill.TryFromName(arg, true, out var skill))
                {
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (skill)
                    {
                        case Farmer.farmingSkill:
                            Game1.player.farmingLevel.Value = 0;
                            break;
                        case Farmer.fishingSkill:
                            Game1.player.fishingLevel.Value = 0;
                            break;
                        case Farmer.foragingSkill:
                            Game1.player.foragingLevel.Value = 0;
                            break;
                        case Farmer.miningSkill:
                            Game1.player.miningLevel.Value = 0;
                            break;
                        case Farmer.combatSkill:
                            Game1.player.combatLevel.Value = 0;
                            break;
                        case Farmer.luckSkill:
                            Game1.player.luckLevel.Value = 0;
                            break;
                    }

                    Game1.player.experiencePoints[skill] = 0;
                    Game1.player.newLevels.Set(Game1.player.newLevels.Where(p => p.X != skill).ToList());
                    if (ModEntry.Config.ForgetRecipes && skill < Skill.Luck)
                        Game1.player.ForgetRecipesForSkill(skill);
                }
                else
                {
                    var customSkill = CustomSkill.LoadedSkills.Values.FirstOrDefault(s =>
                        string.Equals(s.DisplayName, arg, StringComparison.CurrentCultureIgnoreCase));
                    if (customSkill is null)
                    {
                        Log.W($"Ignoring unknown skill {arg}.");
                        continue;
                    }

                    ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(Game1.player, customSkill.StringId,
                        -customSkill.CurrentExp);

                    var newLevels = ExtendedSpaceCoreAPI.GetCustomSkillNewLevels.Value();
                    ExtendedSpaceCoreAPI.SetCustomSkillNewLevels.Value(newLevels
                        .Where(pair => pair.Key != customSkill.StringId).ToList());

                    if (ModEntry.Config.ForgetRecipes &&
                        customSkill.StringId == "blueberry.LoveOfCooking.CookingSkill")
                        Game1.player.ForgetRecipesForLoveOfCookingSkill();
                }
        }
    }
}