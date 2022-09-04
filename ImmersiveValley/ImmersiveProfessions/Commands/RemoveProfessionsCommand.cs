namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Extensions;
using Framework;
using LinqFasterer;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class RemoveProfessionsCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RemoveProfessionsCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "remove_professions", "remove_profs", "remove" };

    /// <inheritdoc />
    public override string Documentation =>
        "Remove the specified professions from the player. Does not affect skill levels." + GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length <= 0)
        {
            Log.W("You must specify at least one profession." + GetUsage());
            return;
        }

        List<int> professionsToRemove = new();
        foreach (var arg in args)
        {
            if (string.Equals(arg, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Profession
                    .GetRange()
                    .Concat(CustomProfession.LoadedProfessions.Values.Select(p => p.Id))
                    .ToArray();

                professionsToRemove.AddRange(range);
                Log.I($"Removed all professions from {Game1.player.Name}.");
                break;
            }

            if (string.Equals(arg, "rogue", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(arg, "unknown", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Game1.player.professions
                    .WhereF(pid =>
                        !Profession.TryFromValue(pid, out _) && CustomProfession.LoadedProfessions.Values.All(p => pid != p.Id))
                    .ToArrayF();

                professionsToRemove.AddRange(range);
                Log.I($"Removed unknown professions from {Game1.player.Name}.");
            }
            else if (Profession.TryFromName(arg, true, out var profession) ||
                     Profession.TryFromLocalizedName(arg, true, out profession))
            {
                professionsToRemove.Add(profession.Id);
                professionsToRemove.Add(profession.Id + 100);
                Log.I($"Removed {profession.StringId} profession from {Game1.player.Name}.");
            }
            else
            {
                var customProfession = CustomProfession.LoadedProfessions.Values.FirstOrDefault(p =>
                    string.Equals(arg, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(arg, p.GetDisplayName().TrimAll(), StringComparison.InvariantCultureIgnoreCase));
                if (customProfession is null)
                {
                    Log.W($"Ignoring unknown profession {arg}.");
                    continue;
                }

                professionsToRemove.Add(customProfession.Id);
                Log.I($"Removed {customProfession.StringId} profession from {Game1.player.Name}.");
            }
        }

        foreach (var pid in professionsToRemove.DistinctF()) GameLocation.RemoveProfession(pid);

        LevelUpMenu.RevalidateHealth(Game1.player);
    }

    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Triggers.FirstF()} [--prestige] <profession1> <profession2> ... <professionN>";
        result += "\n\nParameters:";
        result +=
            "\n\t- <profession>\t- a valid profession name, `all` or `unknown`. Use `unknown` to remove rogue professions from uninstalled custom skill mods.";
        result += "\n\nExamples:";
        result += $"\n\t- {Handler.EntryCommand} {Triggers.FirstF()} artisan brute";
        result += $"\n\t- {Handler.EntryCommand} {Triggers.FirstF()} -p all";
        return result;
    }
}