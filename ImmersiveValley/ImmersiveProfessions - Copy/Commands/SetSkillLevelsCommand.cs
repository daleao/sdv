namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Framework;
using Framework.Utility;
using JetBrains.Annotations;
using StardewValley;
using System;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class SetSkillLevelsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetSkillLevelsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_levels", "set_skills" };

    /// <inheritdoc />
    public override string Documentation =>
        "Set the level of the specified skills. For debug only!! Will not grant recipes or other immediate perks. For a proper level-up use `debug experience` instead." +
        GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length < 2 || args.Length % 2 != 0)
        {
            Log.W("You must provide both a skill name and new level." + GetUsage());
            return;
        }

        if (string.Equals(args[0], "all", StringComparison.InvariantCultureIgnoreCase))
        {
            if (!int.TryParse(args[1], out var newLevel))
            {
                Log.W("New level must be a valid integer." + GetUsage());
                return;
            }

            foreach (var skill in Skill.List)
            {
                var diff = Experience.ExperienceByLevel[newLevel] - skill.CurrentExp;
                Game1.player.gainExperience(skill, diff);
            }

            foreach (var customSkill in ModEntry.CustomSkills.Values.OfType<CustomSkill>())
            {
                var diff = Experience.ExperienceByLevel[newLevel] - customSkill.CurrentExp;
                ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(Game1.player, customSkill.StringId, diff);
            }
        }

        var argsList = args.ToList();
        while (argsList.Count > 0)
        {
            if (!int.TryParse(args[1], out var newLevel))
            {
                Log.W("New level must be a valid integer." + GetUsage());
                return;
            }

            var skillName = args[0];
            if (!Skill.TryFromName(skillName, true, out var skill))
            {
                var found = ModEntry.CustomSkills.Values.SingleOrDefault(s =>
                    string.Equals(s.StringId, skillName, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(s.DisplayName, skillName, StringComparison.CurrentCultureIgnoreCase));
                if (found is not CustomSkill customSkill)
                {
                    Log.W("You must provide a valid skill name." + GetUsage());
                    return;
                }

                var diff = Experience.ExperienceByLevel[newLevel] - customSkill.CurrentExp;
                ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(Game1.player, customSkill.StringId, diff);
            }
            else
            {
                var diff = Experience.ExperienceByLevel[newLevel] - skill.CurrentExp;
                Game1.player.gainExperience(skill, diff);
            }

            argsList.RemoveAt(0);
            argsList.RemoveAt(0);
        }
    }

    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Triggers.First()} <skill1> <newLevel> <skill2> <newLevel> ...";
        result += "\n\nParameters:";
        result += "\n\t- <skill>\t- a valid skill name, or 'all'";
        result += "\n\t- <newLevel>\t- a valid integer level";
        result += "\n\nExamples:";
        result += $"\n\t- {Handler.EntryCommand} {Triggers.First()} farming 5 cooking 10";
        return result;
    }
}