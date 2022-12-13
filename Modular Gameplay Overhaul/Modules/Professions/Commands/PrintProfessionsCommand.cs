namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

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
    public override void Callback(string[] args)
    {
        if (Game1.player.professions.Count == 0)
        {
            Log.I($"Farmer {Game1.player.Name} doesn't have any professions.");
            return;
        }

        var message = $"Farmer {Game1.player.Name}'s professions:";
        foreach (var pid in Game1.player.professions)
        {
            string name;
            if (Profession.TryFromValue(pid > 100 ? pid - 100 : pid, out var profession))
            {
                name = profession.StringId + (pid > 100 ? " (P)" : Empty);
            }
            else if (SCProfession.Loaded.ContainsKey(pid))
            {
                name = SCProfession.Loaded[pid].StringId + " (" + SCProfession.Loaded[pid].Skill.StringId + ')';
            }
            else
            {
                name = $"Unknown profession {pid}";
            }

            message += "\n\t- " + name;
        }

        Log.I(message);
    }
}
