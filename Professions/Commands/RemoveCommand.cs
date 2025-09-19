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

            Log.I($"Cleared all professions from {Game1.player.Name}.");
            return true;
        }

        if (args.Length == 0)
        {
            Log.W("You must specify at least one profession.");
            return false;
        }

        var tokens = args.ToList();
        var farmerIndex = 1;
        var farmerArgs = tokens.Where(a => a.ToLower() is "--farmer" or "-f").ToList();
        if (farmerArgs.Any())
        {
            var fIndex = tokens.IndexOf(farmerArgs.First());
            if (fIndex != -1 && tokens.Count > fIndex + 1 && int.TryParse(tokens[fIndex + 1], out var parsed))
            {
                farmerIndex = parsed;
            }
            else
            {
                Log.W("The `--farmer` flag is missing an accompanying farmer index. Please specify \"1\" for player 1 or \"2\" for player 2.");
                return false;
            }

            tokens.RemoveAt(fIndex + 1);
            tokens.RemoveAt(fIndex);
        }

        var player = Game1.player;
        if (farmerIndex > 1)
        {
            if (!Context.IsSplitScreen)
            {
                Log.W("Can't assign professions to co-op players in a non-splitscreen session.");
                return false;
            }

            var screenId = farmerIndex - 1;
            var onlinePlayers = ModHelper.Multiplayer.GetConnectedPlayers().ToList();
            if (screenId > onlinePlayers.Count)
            {
                Log.W($"Insufficient online players for setting specified player \"{farmerIndex}\".");
                return false;
            }

            var multiplayerId = onlinePlayers.Find(peer => peer.ScreenID == screenId)?.PlayerID;
            if (multiplayerId is null)
            {
                Log.W($"Couldn't find online player with the desired player screen ID \"{screenId}\".");
                return false;
            }

            player = Game1.GetPlayer(multiplayerId.Value, onlyOnline: true);
            if (player is null)
            {
                Log.W($"Couldn't find online player with specified player screen ID \"{screenId}\".");
                return false;
            }
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

                        player.stats.Set(StatKeys.Mastery(skill1), 0);
                        Log.I($"Unmastered the {skill1} skill.");
                    }

                    player.stats.Set(StatKeys.MasteryExp, 0);
                    player.stats.Set(StatKeys.MasteryLevelsSpent, 0);
                    return true;
                }

                if (Skill.TryFromName(arg, true, out var skill2))
                {
                    if (!skill2.CanGainPrestigeLevels())
                    {
                        Log.I($"{skill2} skill has not been mastered.");
                        return true;
                    }

                    player.stats.Set(StatKeys.Mastery(skill2), 0);
                    player.stats.Set(
                        StatKeys.MasteryExp,
                        Math.Max(MasteryTrackerMenu.getMasteryExpNeededForLevel(MasteryTrackerMenu.getCurrentMasteryLevel() - 1), 0));
                    player.stats.Set(StatKeys.MasteryLevelsSpent, Math.Max(Game1.player.stats.Get(StatKeys.MasteryLevelsSpent) - 1, 0));
                    Log.I($"Unmastered the {skill2} skill.");
                }
                else
                {
                    Log.I($"Ignoring unknown vanilla skill \"{skill2}\".");
                }
            }
        }

        List<int> professionsToRemove = [];
        foreach (var arg in args)
        {
            if (string.Equals(arg, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var shouldInvalidate = player.professions.Intersect(Profession.GetRange(true)).Any();
                player.professions.Clear();
                LevelUpMenu.RevalidateHealth(player);
                if (shouldInvalidate)
                {
                    ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
                }

                Log.I($"Removed all professions from {player.Name}.");
                break;
            }

            if (string.Equals(arg, "rogue", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(arg, "unknown", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = player.professions
                    .Where(pid =>
                        !Profession.TryFromValue(pid, out _) && !Profession.TryFromValue(pid + 100, out _) &&
                        CustomProfession.List.All(p => pid != p.Id && pid != p.Id + 100))
                    .ToArray();

                professionsToRemove.AddRange(range);
                Log.I($"Removed unknown professions from {player.Name}.");
            }
            else if (Profession.TryFromName(arg, true, out var profession) ||
                     Profession.TryFromLocalizedName(arg, true, out profession) ||
                     (int.TryParse(arg, out var id) && Profession.TryFromValue(id, out profession)))
            {
                professionsToRemove.Add(profession.Id);
                professionsToRemove.Add(profession.Id + 100);
                Log.I($"Removed {profession.StringId} profession from {player.Name}.");
            }
            else
            {
                var customProfession = CustomProfession.List.FirstOrDefault(p =>
                    string.Equals(arg, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(arg, p.Title.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    (int.TryParse(arg, out id) && id == p.Id));
                if (customProfession is null)
                {
                    Log.W($"Ignoring unknown profession {arg}.");
                    continue;
                }

                professionsToRemove.Add(customProfession.Id);
                Log.I($"Removed {customProfession.StringId} profession from {Game1.player.Name}.");
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
