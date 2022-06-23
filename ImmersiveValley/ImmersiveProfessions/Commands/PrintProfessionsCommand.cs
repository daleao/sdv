namespace DaLion.Stardew.Professions.Commands;

#region using directives

using static System.String;

using StardewValley;

using Common;
using Common.Commands;
using Framework;

#endregion using directives

internal class PrintProfessionsCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "professions";

    /// <inheritdoc />
    public string Documentation => "List the current professions.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        if (!Game1.player.professions.Any())
        {
            Log.I($"Farmer {Game1.player.Name} doesn't have any professions.");
            return;
        }

        var message = $"Farmer {Game1.player.Name}'s professions:";
        foreach (var pid in Game1.player.professions)
        {
            string name;
            if (Profession.TryFromValue(pid > 100 ? pid - 100 : pid, out var profession))
                name = profession.StringId + (pid > 100 ? " (P)" : Empty);
            else if (ModEntry.CustomProfessions.ContainsKey(pid))
                name = ModEntry.CustomProfessions[pid].StringId;
            else
                name = $"Unknown profession {pid}";

            message += "\n\t- " + name;
        }

        Log.I(message);
    }
}