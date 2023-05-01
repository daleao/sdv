namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Collections;

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
    public override string[] Triggers { get; } = { "set_levels", "set_skills", "levelup", "skillup" };

    /// <inheritdoc />
    public override string Documentation =>
        "For debug only!! Set the level of the specified skills. Will not grant recipes or other immediate perks. For a proper level-up use `debug experience` instead." +
        this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
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

            if (newLevel < 0)
            {
                Log.W("New level must be greater than zero." + this.GetUsage());
                return;
            }

            Skill.List.ForEach(s => s.SetLevel(newLevel));
            SCSkill.Loaded.Values.ForEach(s => s.SetLevel(newLevel));
        }

        var argsList = args.ToList();
        while (argsList.Count > 0)
        {
            if (!int.TryParse(args[1], out var newLevel))
            {
                Log.W("New level must be a valid integer." + this.GetUsage());
                return;
            }

            if (newLevel < 0)
            {
                Log.W("New level must be greater than zero." + this.GetUsage());
                return;
            }

            var skillName = args[0];
            if (!Skill.TryFromName(skillName, true, out var skill))
            {
                var found = SCSkill.Loaded.Values.FirstOrDefault(s =>
                    string.Equals(s.StringId, skillName, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(s.DisplayName, skillName, StringComparison.CurrentCultureIgnoreCase));
                if (found is not SCSkill customSkill)
                {
                    Log.W("You must provide a valid skill name." + this.GetUsage());
                    return;
                }

                customSkill.SetLevel(newLevel);
            }
            else
            {
                skill.SetLevel(newLevel);
            }

            argsList.RemoveAt(0);
            argsList.RemoveAt(0);
        }
    }

    private string GetUsage()
    {
        var result =
            new StringBuilder(
                $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <skill1> <newLevel> <skill2> <newLevel> ...");
        result.Append("\n\nParameters:");
        result.Append("\n\t- <skill>\t- a valid skill name, or 'all'");
        result.Append("\n\t- <newLevel>\t- a valid integer level");
        result.Append("\n\nExamples:");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} farming 5 cooking 10");
        return result.ToString();
    }
}
