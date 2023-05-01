namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

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
    public override string[] Triggers { get; } = { "print_professions", "print_profs", "professions", "profs", "list" };

    /// <inheritdoc />
    public override string Documentation => "List the player's current professions.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (Game1.player.professions.Count == 0)
        {
            Log.I($"Farmer {Game1.player.Name} doesn't have any professions.");
            return;
        }

        var message = new StringBuilder($"Farmer {Game1.player.Name}'s professions:");
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

            message.Append("\n\t- ").Append(name);
        }

        Log.I(message.ToString());
    }
}
