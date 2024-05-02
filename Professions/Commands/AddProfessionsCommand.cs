namespace DaLion.Professions.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
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
    public override string[] Triggers { get; } = ["add", "get"];

    /// <inheritdoc />
    public override string Documentation => "Add the specified professions to the player. Does not affect skill levels.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            this.Handler.Log.W("You must specify at least one profession.");
            return false;
        }

        var prestige = args.Any(a => a is "-p" or "--prestiged");
        if (prestige)
        {
            args = args.Except(["-p", "--prestiged"]).ToArray();
        }

        List<int> professionsToAdd = [];
        foreach (var arg in args)
        {
            if (string.Equals(arg, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Profession.GetRange().ToArray();
                if (prestige)
                {
                    range = range.Concat(Profession.GetRange(true)).ToArray();
                }

                range = [.. range, .. CustomProfession.List.Select(p => p.Id)];
                professionsToAdd.AddRange(range);
                this.Handler.Log.I($"Added all {(prestige ? "prestiged " : string.Empty)}professions to {Game1.player.Name}.");
                break;
            }

            if (Profession.TryFromName(arg, true, out var profession) ||
                Profession.TryFromLocalizedName(arg, true, out profession))
            {
                if ((!prestige && Game1.player.HasProfession(profession)) ||
                    (prestige && Game1.player.HasProfession(profession, true)))
                {
                    this.Handler.Log.W($"Farmer {Game1.player.Name} already has the {profession.StringId} profession.");
                    continue;
                }

                professionsToAdd.Add(profession.Id);
                if (prestige)
                {
                    professionsToAdd.Add(profession + 100);
                }

                this.Handler.Log.I($"Added {profession.StringId}{(prestige ? " (P)" : string.Empty)} profession to {Game1.player.Name}.");
            }
            else
            {
                var customProfession = CustomProfession.List.FirstOrDefault(p =>
                    string.Equals(arg, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(arg, p.Title.TrimAll(), StringComparison.InvariantCultureIgnoreCase));
                if (customProfession is null)
                {
                    this.Handler.Log.W($"{arg} is not a valid profession name.");
                    continue;
                }

                if (prestige)
                {
                    this.Handler.Log.W($"Cannot prestige custom skill profession {customProfession.StringId}.");
                    continue;
                }

                if (Game1.player.HasProfession(customProfession))
                {
                    this.Handler.Log.W($"Farmer {Game1.player.Name} already has the {customProfession.StringId} profession.");
                    continue;
                }

                professionsToAdd.Add(customProfession.Id);
                this.Handler.Log.I($"Added the {customProfession.StringId} profession to {Game1.player.Name}.");
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
        if (professionsToAdd.Intersect(Profession.GetRange(true)).Any())
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }

        return true;
    }

    /// <inheritdoc />
    protected override string GetUsage()
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
