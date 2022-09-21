namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System;
using System.Linq;
using DaLion.Common;
using DaLion.Common.Commands;
using DaLion.Stardew.Professions.Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class SetSkillLevelsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SetSkillLevelsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetSkillLevelsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_levels", "set_skills" };

    /// <inheritdoc />
    public override string Documentation =>
        "For debug only!! Set the level of the specified skills. Will not grant recipes or other immediate perks. For a proper level-up use `debug experience` instead." +
        this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length < 2 || args.Length % 2 != 0)
        {
            Log.W("You must provide both a skill name and new level." + this.GetUsage());
            return;
        }

        if (string.Equals(args[0], "all", StringComparison.InvariantCultureIgnoreCase))
        {
            if (!int.TryParse(args[1], out var newLevel))
            {
                Log.W("New level must be a valid integer." + this.GetUsage());
                return;
            }

            if (newLevel < 0 || newLevel > ISkill.MaxLevel)
            {
                Log.W($"New level must be within the valid range of skills levels [0, {ISkill.MaxLevel}]." +
                      this.GetUsage());
                return;
            }

            foreach (var skill in Skill.List)
            {
                var diff = ISkill.ExperienceByLevel[newLevel] - skill.CurrentExp;
                Game1.player.gainExperience(skill, diff);
            }

            foreach (var customSkill in CustomSkill.Loaded.Values.OfType<CustomSkill>())
            {
                var diff = ISkill.ExperienceByLevel[newLevel] - customSkill.CurrentExp;
                ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(Game1.player, customSkill.StringId, diff);
            }
        }

        var argsList = args.ToList();
        while (argsList.Count > 0)
        {
            if (!int.TryParse(args[1], out var newLevel))
            {
                Log.W("New level must be a valid integer." + this.GetUsage());
                return;
            }

            if (newLevel < 0 || newLevel > ISkill.MaxLevel)
            {
                Log.W($"New level must be within the valid range of skills levels [0, {ISkill.MaxLevel}]." +
                      this.GetUsage());
                return;
            }

            var skillName = args[0];
            if (!Skill.TryFromName(skillName, true, out var skill))
            {
                var found = CustomSkill.Loaded.Values.FirstOrDefault(s =>
                    string.Equals(s.StringId, skillName, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(s.DisplayName, skillName, StringComparison.CurrentCultureIgnoreCase));
                if (found is not CustomSkill customSkill)
                {
                    Log.W("You must provide a valid skill name." + this.GetUsage());
                    return;
                }

                var diff = ISkill.ExperienceByLevel[newLevel] - customSkill.CurrentExp;
                ModEntry.SpaceCoreApi!.AddExperienceForCustomSkill(Game1.player, customSkill.StringId, diff);
            }
            else
            {
                var diff = ISkill.ExperienceByLevel[newLevel] - skill.CurrentExp;
                Game1.player.gainExperience(skill, diff);
            }

            argsList.RemoveAt(0);
            argsList.RemoveAt(0);
        }
    }

    private string GetUsage()
    {
        var result =
            $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers.First()} <skill1> <newLevel> <skill2> <newLevel> ...";
        result += "\n\nParameters:";
        result += "\n\t- <skill>\t- a valid skill name, or 'all'";
        result += "\n\t- <newLevel>\t- a valid integer level";
        result += "\n\nExamples:";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} farming 5 cooking 10";
        return result;
    }
}
