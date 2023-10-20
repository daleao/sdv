namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Shared.Commands;
using static System.String;

#endregion using directives

[UsedImplicitly]
internal sealed class PrintProfessionsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="PrintProfessionsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal PrintProfessionsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "list" };

    /// <inheritdoc />
    public override string Documentation => "List the player's current professions. Or, alternatively, list the professions in available to the specified skill.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        StringBuilder sb;
        if (args.Length > 0)
        {
            var skillName = args[0];
            ISkill skill;
            if (!Skill.TryFromName(skillName, true, out var vanillaSkill))
            {
                var found = SCSkill.Loaded.Values.FirstOrDefault(s =>
                    string.Equals(s.StringId, skillName, StringComparison.CurrentCultureIgnoreCase) ||
                    string.Equals(s.DisplayName, skillName, StringComparison.CurrentCultureIgnoreCase));
                if (found is not SCSkill customSkill)
                {
                    Log.W($"{args[0]} is not a valid skill name.");
                    return;
                }

                skill = customSkill;
            }
            else
            {
                skill = vanillaSkill;
            }

            sb = new StringBuilder($"Professions in {skill.StringId}:");
            for (var i = 0; i < 2; i++)
            {
                var profession = skill.Professions[i];
                sb.Append($"\n\t- {profession.StringId} (ID: {profession.Id})");
                foreach (var branch in profession.BranchingProfessions)
                {
                    sb.Append($"\n\t\t- {branch.StringId} (ID: {branch.Id})");
                }
            }

            Log.I(sb.ToString());
            return;
        }

        if (Game1.player.professions.Count == 0)
        {
            Log.I($"Farmer {Game1.player.Name} doesn't have any professions.");
            return;
        }

        sb = new StringBuilder($"Farmer {Game1.player.Name}'s professions:");
        for (var i = 0; i < Game1.player.professions.Count; i++)
        {
            var pid = Game1.player.professions[i];
            var name = new StringBuilder();
            if (Profession.TryFromValue(pid >= 100 ? pid - 100 : pid, out var profession))
            {
                name.Append(profession.StringId + (pid >= 100 ? " (P)" : Empty));
            }
            else if (SCProfession.Loaded.TryGetValue(pid, out var scProfession) || SCProfession.Loaded.TryGetValue(pid - 100, out scProfession))
            {
                name.Append(scProfession.StringId);
                if (!SCProfession.Loaded.ContainsKey(pid))
                {
                    name.Append(" (P)");
                }

                name.Append(" (" + scProfession.Skill.StringId + ')');
            }
            else
            {
                name.Append($"Unknown profession {pid}");
            }

            sb.Append("\n\t- ").Append(name);
        }

        Log.I(sb.ToString());
    }
}
