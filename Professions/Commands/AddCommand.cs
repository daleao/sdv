namespace DaLion.Professions.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaLion.Professions.Framework;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Collections;
using DaLion.Shared.Extensions.SMAPI;
using StardewValley.Constants;
using StardewValley.Menus;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="AddCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class AddCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["add", "get"];

    /// <inheritdoc />
    public override string Documentation =>
        "Add the specified professions to the player without affecting skill levels. Can also be used to add masteries to the specified skills using the keyword \"mastery\".";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length == 0)
        {
            this.Handler.Log.W("You must specify at least one profession.");
            return false;
        }

        if (args[0].ToLower() is "mastery" or "masteries")
        {
            args = args.Skip(1).ToArray();
            foreach (var arg in args)
            {
                if (string.Equals(arg, "all", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var skill1 in Skill.List)
                    {
                        if (skill1.CanGainPrestigeLevels())
                        {
                            continue;
                        }

                        Game1.player.stats.Set(StatKeys.Mastery(skill1), 1);
                        this.Handler.Log.I($"Mastered the {skill1} skill.");
                    }

                    Game1.player.stats.Set(StatKeys.MasteryExp, MasteryTrackerMenu.getMasteryExpNeededForLevel(5));
                    return true;
                }

                if (Skill.TryFromName(arg, true, out var skill2))
                {
                    if (skill2.CanGainPrestigeLevels())
                    {
                        this.Handler.Log.I($"{skill2} skill is already mastered.");
                        return true;
                    }

                    Game1.player.stats.Set(StatKeys.Mastery(skill2), 1);
                    Game1.player.stats.Set(
                        StatKeys.MasteryExp,
                        MasteryTrackerMenu.getMasteryExpNeededForLevel(MasteryTrackerMenu.getCurrentMasteryLevel() + 1));
                    this.Handler.Log.I($"Mastered the {skill2} skill.");
                }
                else
                {
                    this.Handler.Log.I($"Ignoring unknown vanilla skill \"{skill2}\".");
                }
            }
        }

        var prestigeArgs = args.Where(a => a.ToLower() is "-p" or "--prestiged").ToArray();
        var prestige = prestigeArgs.Any();
        if (prestige)
        {
            args = args.Except(prestigeArgs).ToArray();
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
                this.Handler.Log.I(
                    $"Added all {(prestige ? "prestiged " : string.Empty)}professions to {Game1.player.Name}.");
                break;
            }

            if (Profession.TryFromName(arg, true, out var profession) ||
                Profession.TryFromLocalizedName(arg, true, out profession) ||
                (int.TryParse(arg, out var id) && Profession.TryFromValue(id, out profession)))
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

                this.Handler.Log.I(
                    $"Added {profession.StringId}{(prestige ? " (P)" : string.Empty)} profession to {Game1.player.Name}.");
            }
            else
            {
                var customProfession = CustomProfession.List.FirstOrDefault(p =>
                    string.Equals(arg, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(arg, p.Title.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    (int.TryParse(arg, out id) && id == p.Id));
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
                    this.Handler.Log.W(
                        $"Farmer {Game1.player.Name} already has the {customProfession.StringId} profession.");
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
        var sb =
            new StringBuilder(
                $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} [--prestige / -p] <profession1> <profession2> ... <professionN>");
        sb.Append("\n\nParameters:");
        sb.Append("\n\t- <profession>\t- a valid profession name, or `all`");
        sb.Append("\n\nOptional flags:");
        sb.Append(
            "\n\t-prestige, -p\t- add the prestiged versions of the specified professions (base versions will be added automatically if needed)");
        sb.Append("\n\nExamples:");
        sb.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} artisan brute");
        sb.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} -p all");

        sb.Append($"\n\nAlternative usage: {this.Handler.EntryCommand} {this.Triggers[0]} mastery <skill1> <skill2> ... <skillN>");
        sb.Append("\n\nParameters:");
        sb.Append("\n\t- <skill>\t- a valid vanilla skill name, or `all`");
        sb.Append("\n\nExamples:");
        sb.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} mastery farming");
        return sb.ToString();
    }
}
