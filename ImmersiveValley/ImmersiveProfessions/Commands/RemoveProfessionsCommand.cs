namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using StardewValley;
using StardewValley.Menus;

using Common;
using Common.Commands;
using Common.Extensions;
using Framework;

#endregion using directives

internal class RemoveProfessionsCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "remove_professions";

    /// <inheritdoc />
    public string Documentation => "Remove the specified professions. Does not affect skill levels." + GetUsage();

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        if (!args.Any())
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
                    .Concat(ModEntry.CustomProfessions.Values.Select(p => p.Id))
                    .ToArray();

                professionsToRemove.AddRange(range);
                Log.I($"Removed all professions from {Game1.player.Name}.");
                break;
            }

            if (string.Equals(arg, "rogue", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(arg, "unknown", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Game1.player.professions
                    .Where(pid =>
                        !Profession.TryFromValue(pid, out _) && ModEntry.CustomProfessions.Values.All(p => pid != p.Id))
                    .ToArray();

                professionsToRemove.AddRange(range);
                Log.I($"Removed unknown professions from {Game1.player.Name}.");
            }
            else if (Profession.TryFromName(arg, true, out var profession) ||
                     Profession.TryFromLocalizedName(arg, true, out profession))
            {
                professionsToRemove.Add(profession.Id);
                professionsToRemove.Add(profession.Id + 100);
                Log.I($"Removed the {profession.StringId} profession from {Game1.player.Name}.");
            }
            else
            {
                var customProfession = ModEntry.CustomProfessions.Values.SingleOrDefault(p =>
                    string.Equals(arg, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(arg, p.GetDisplayName().TrimAll(), StringComparison.InvariantCultureIgnoreCase));
                if (customProfession is null)
                {
                    Log.W($"Ignoring unknown profession {arg}.");
                    continue;
                }

                professionsToRemove.Add(customProfession.Id);
                Log.I($"Removed the {profession.StringId} profession from {Game1.player.Name}.");
            }
        }

        foreach (var pid in professionsToRemove.Distinct())
        {
            Game1.player.professions.Remove(pid);
            LevelUpMenu.removeImmediateProfessionPerk(pid);
        }

        LevelUpMenu.RevalidateHealth(Game1.player);
    }

    private static string GetUsage()
    {
        var result = "\n\nUsage: remove_professions [--prestige] <profession1> <profession2> ... <professionN>";
        result += "\n\nParameters:";
        result +=
            "\n\t<profession>\t- a valid profession name, `all` or `unknown`. Use `unknown` to remove rogue professions from uninstalled custom skill mods.";
        result += "\n\nOptional flags:";
        result +=
            "\n\t--prestige, -p\t- add the prestiged versions of the specified professions (will automatically add the base versions as well)";
        result += "\n\nExamples:";
        result += "\n\tplayer_addprofessions artisan brute";
        result += "\n\tplayer_addprofessions -p all";
        return result;
    }
}