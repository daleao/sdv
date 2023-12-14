namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
using Shared.Extensions.Collections;
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
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("You must specify at least one profession." + this.GetUsage());
            return;
        }

        var prestige = args.Any(a => a is "-p" or "--prestiged");
        if (prestige)
        {
            args = args.Except(new[] { "-p", "--prestiged" }).ToArray();
        }

        List<int> professionsToAdd = new();
        for (var i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = VanillaProfession.GetRange().ToArray();
                if (prestige)
                {
                    range = range.Concat(VanillaProfession.GetRange(true)).ToArray();
                }

                range = range
                    .Concat(CustomProfession.List.Select(p => p.Id))
                    .ToArray();
                professionsToAdd.AddRange(range);
                Log.I($"Added all {(prestige ? "prestiged " : string.Empty)}professions to {Game1.player.Name}.");
                break;
            }

            if (VanillaProfession.TryFromName(args[i], true, out var profession) ||
                VanillaProfession.TryFromLocalizedName(args[i], true, out profession))
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
                var customProfession = CustomProfession.List.FirstOrDefault(p =>
                    string.Equals(args[i], p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(args[i], p.Title.TrimAll(), StringComparison.InvariantCultureIgnoreCase));
                if (customProfession is null)
                {
                    Log.W($"Ignoring unknown profession {args[i]}.");
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
            if (Game1.player.professions.AddOrReplace(pid))
            {
                levelUpMenu.getImmediateProfessionPerk(pid);
            }
        }

        LevelUpMenu.RevalidateHealth(Game1.player);
        if (professionsToAdd.Intersect(VanillaProfession.GetRange(true)).Any())
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }

    private string GetUsage()
    {
        var result =
            new StringBuilder(
                $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} [--prestige / -p] <profession1> <profession2> ... <professionN>");
        result.Append("\n\nParameters:");
        result.Append("\n\t- <profession>\t- a valid profession name, or `all`");
        result.Append("\n\nOptional flags:");
        result.Append(
            "\n\t-prestige, -p\t- add the prestiged versions of the specified professions (base versions will be added automatically if needed)");
        result.Append("\n\nExamples:");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} artisan brute");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} -p all");
        return result.ToString();
    }
}
