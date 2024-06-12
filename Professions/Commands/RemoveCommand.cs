namespace DaLion.Professions.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
using StardewValley.Constants;
using StardewValley.Menus;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="RemoveCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class RemoveCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["remove", "clear"];

    /// <inheritdoc />
    public override string Documentation =>
        "Remove the specified professions from the player without affecting skill levels. Can also be used to remove masteries from the specified skills using the keyword \"mastery\".";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (trigger == "clear")
        {
            var shouldInvalidate = Game1.player.professions.Intersect(Profession.GetRange(true)).Any();
            Game1.player.professions.Clear();
            LevelUpMenu.RevalidateHealth(Game1.player);
            if (shouldInvalidate)
            {
                ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
            }

            this.Handler.Log.I($"Cleared all professions from {Game1.player.Name}.");
            return true;
        }

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

                        Game1.player.stats.Set(StatKeys.Mastery(skill1), 0);
                        this.Handler.Log.I($"Unmastered the {skill1} skill.");
                    }

                    Game1.player.stats.Set(StatKeys.MasteryExp, 0);
                    return true;
                }

                if (Skill.TryFromName(arg, true, out var skill2))
                {
                    if (!skill2.CanGainPrestigeLevels())
                    {
                        this.Handler.Log.I($"{skill2} skill has not been mastered.");
                        return true;
                    }

                    Game1.player.stats.Set(StatKeys.Mastery(skill2), 0);
                    Game1.player.stats.Set(
                        StatKeys.MasteryExp,
                        MasteryTrackerMenu.getMasteryExpNeededForLevel(MasteryTrackerMenu.getCurrentMasteryLevel() - 1));
                    this.Handler.Log.I($"Unmastered the {skill2} skill.");
                }
                else
                {
                    this.Handler.Log.I($"Ignoring unknown vanilla skill \"{skill2}\".");
                }
            }
        }

        List<int> professionsToRemove = [];
        foreach (var arg in args)
        {
            if (string.Equals(arg, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var shouldInvalidate = Game1.player.professions.Intersect(Profession.GetRange(true)).Any();
                Game1.player.professions.Clear();
                LevelUpMenu.RevalidateHealth(Game1.player);
                if (shouldInvalidate)
                {
                    ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
                }

                this.Handler.Log.I($"Removed all professions from {Game1.player.Name}.");
                break;
            }

            if (string.Equals(arg, "rogue", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(arg, "unknown", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Game1.player.professions
                    .Where(pid =>
                        !Profession.TryFromValue(pid, out _) && !Profession.TryFromValue(pid + 100, out _) &&
                        CustomProfession.List.All(p => pid != p.Id && pid != p.Id + 100))
                    .ToArray();

                professionsToRemove.AddRange(range);
                this.Handler.Log.I($"Removed unknown professions from {Game1.player.Name}.");
            }
            else if (Profession.TryFromName(arg, true, out var profession) ||
                     Profession.TryFromLocalizedName(arg, true, out profession) ||
                     (int.TryParse(arg, out var id) && Profession.TryFromValue(id, out profession)))
            {
                professionsToRemove.Add(profession.Id);
                professionsToRemove.Add(profession.Id + 100);
                this.Handler.Log.I($"Removed {profession.StringId} profession from {Game1.player.Name}.");
            }
            else
            {
                var customProfession = CustomProfession.List.FirstOrDefault(p =>
                    string.Equals(arg, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(arg, p.Title.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    (int.TryParse(arg, out id) && id == p.Id));
                if (customProfession is null)
                {
                    this.Handler.Log.W($"Ignoring unknown profession {arg}.");
                    continue;
                }

                professionsToRemove.Add(customProfession.Id);
                this.Handler.Log.I($"Removed {customProfession.StringId} profession from {Game1.player.Name}.");
            }
        }

        foreach (var pid in professionsToRemove.Distinct())
        {
            GameLocation.RemoveProfession(pid);
        }

        LevelUpMenu.RevalidateHealth(Game1.player);
        if (professionsToRemove.Intersect(Profession.GetRange(true)).Any())
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
                $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} [--prestige] <profession1> <profession2> ... <professionN>");
        sb.Append("\n\nParameters:");
        sb.Append(
            "\n\t- <profession>\t- a valid profession name, `all` or `unknown`. Use `unknown` to remove rogue professions from uninstalled custom skill mods.");
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
