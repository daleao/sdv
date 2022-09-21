namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using DaLion.Common;
using DaLion.Common.Commands;
using DaLion.Common.Extensions;
using DaLion.Stardew.Professions.Extensions;
using DaLion.Stardew.Professions.Framework;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class AddProfessionsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="AddProfessionsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal AddProfessionsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "add_professions", "add_profs", "add" };

    /// <inheritdoc />
    public override string Documentation =>
        "Add the specified professions to the player. Does not affect skill levels." + this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length <= 0)
        {
            Log.W("You must specify at least one profession." + this.GetUsage());
            return;
        }

        var prestige = args.Any(a => a is "-p" or "-prestiged");
        if (prestige)
        {
            args = args.Except(new[] { "-p", "-prestiged" }).ToArray();
        }

        List<int> professionsToAdd = new();
        foreach (var arg in args)
        {
            if (string.Equals(arg, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Profession.GetRange().ToArray();
                if (prestige)
                {
                    range = range.Concat(Enumerable.Range(100, 30)).ToArray();
                }

                range = range.Concat(CustomProfession.LoadedProfessions.Values.Select(p => p.Id)).ToArray();

                professionsToAdd.AddRange(range);
                Log.I($"Added all {(prestige ? "prestiged " : string.Empty)}professions to {Game1.player.Name}.");
                break;
            }

            if (Profession.TryFromName(arg, true, out var profession) ||
                Profession.TryFromLocalizedName(arg, true, out profession))
            {
                if ((!prestige && Game1.player.HasProfession(profession)) ||
                    (prestige && Game1.player.HasProfession(profession, true)))
                {
                    Log.W($"Farmer {Game1.player.Name} already has the {profession.StringId} profession.");
                    continue;
                }

                professionsToAdd.Add(profession.Id);
                if (prestige)
                {
                    professionsToAdd.Add(profession + 100);
                }

                Log.I($"Added {profession.StringId}{(prestige ? " (P)" : string.Empty)} profession to {Game1.player.Name}.");
            }
            else
            {
                var customProfession = CustomProfession.LoadedProfessions.Values.FirstOrDefault(p =>
                    string.Equals(arg, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(arg, p.DisplayName.TrimAll(), StringComparison.InvariantCultureIgnoreCase));
                if (customProfession is null)
                {
                    Log.W($"Ignoring unknown profession {arg}.");
                    continue;
                }

                if (prestige)
                {
                    Log.W($"Cannot prestige custom skill profession {customProfession.StringId}.");
                    continue;
                }

                if (Game1.player.HasProfession(customProfession))
                {
                    Log.W($"Farmer {Game1.player.Name} already has the {customProfession.StringId} profession.");
                    continue;
                }

                professionsToAdd.Add(customProfession.Id);
                Log.I($"Added the {customProfession.StringId} profession to {Game1.player.Name}.");
            }
        }

        LevelUpMenu levelUpMenu = new();
        foreach (var pid in professionsToAdd.Distinct().Except(Game1.player.professions))
        {
            Game1.player.professions.Add(pid);
            levelUpMenu.getImmediateProfessionPerk(pid);
        }

        LevelUpMenu.RevalidateHealth(Game1.player);
    }

    private string GetUsage()
    {
        var result =
            $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers.First()} [--prestige / -p] <profession1> <profession2> ... <professionN>";
        result += "\n\nParameters:";
        result += "\n\t- <profession>\t- a valid profession name, or `all`";
        result += "\n\nOptional flags:";
        result +=
            "\n\t-prestige, -p\t- add the prestiged versions of the specified professions (base versions will be added automatically if needed)";
        result += "\n\nExamples:";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} artisan brute";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} -p all";
        return result;
    }
}
