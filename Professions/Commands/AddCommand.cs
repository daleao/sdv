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

            player = Game1.getFarmer(multiplayerId.Value);
            if (player is null)
            {
                Log.W($"Couldn't find online player with specified player screen ID \"{screenId}\".");
                return false;
            }
        }

        if (tokens[0].ToLower() is "mastery" or "masteries")
        {
            tokens = tokens.Skip(1).ToList();
            foreach (var token in tokens)
            {
                if (string.Equals(token, "all", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var skill1 in Skill.List)
                    {
                        if (skill1.CanGainPrestigeLevels())
                        {
                            continue;
                        }

                        player.stats.Set(StatKeys.Mastery(skill1), 1);
                        Log.I($"Mastered the {skill1} skill.");
                    }

                    player.stats.Set(StatKeys.MasteryExp, MasteryTrackerMenu.getMasteryExpNeededForLevel(5));
                    return true;
                }

                if (Skill.TryFromName(token, true, out var skill2))
                {
                    if (skill2.CanGainPrestigeLevels())
                    {
                        Log.I($"{skill2} skill is already mastered.");
                        return true;
                    }

                    player.stats.Set(StatKeys.Mastery(skill2), 1);
                    player.stats.Set(
                        StatKeys.MasteryExp,
                        MasteryTrackerMenu.getMasteryExpNeededForLevel(MasteryTrackerMenu.getCurrentMasteryLevel() + 1));
                    Log.I($"Mastered the {skill2} skill.");
                }
                else
                {
                    Log.I($"Ignoring unknown vanilla skill \"{skill2}\".");
                }
            }
        }

        var prestigeArgs = tokens.Where(a => a.ToLower() is "--prestiged" or "-p").ToList();
        var prestige = prestigeArgs.Any();
        if (prestige)
        {
            tokens = tokens.Except(prestigeArgs).ToList();
        }

        List<int> professionsToAdd = [];
        foreach (var token in tokens)
        {
            if (string.Equals(token, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Profession.GetRange().ToArray();
                if (prestige)
                {
                    range = range.Concat(Profession.GetRange(true)).ToArray();
                }

                range = [.. range, .. CustomProfession.List.Select(p => p.Id)];
                professionsToAdd.AddRange(range);
                Log.I(
                    $"Added all {(prestige ? "prestiged " : string.Empty)}professions to {player.Name}.");
                break;
            }

            if (Profession.TryFromName(token, true, out var profession) ||
                Profession.TryFromLocalizedName(token, true, out profession) ||
                (int.TryParse(token, out var id) && Profession.TryFromValue(id, out profession)))
            {
                if ((!prestige && player.HasProfession(profession)) ||
                    (prestige && player.HasProfession(profession, true)))
                {
                    Log.W($"Farmer {player.Name} already has the {profession.StringId} profession.");
                    continue;
                }

                professionsToAdd.Add(profession.Id);
                if (prestige)
                {
                    professionsToAdd.Add(profession + 100);
                }

                Log.I(
                    $"Added {profession.StringId}{(prestige ? " (P)" : string.Empty)} profession to {player.Name}.");
            }
            else
            {
                var customProfession = CustomProfession.List.FirstOrDefault(p =>
                    string.Equals(token, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(token, p.Title.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    (int.TryParse(token, out id) && id == p.Id));
                if (customProfession is null)
                {
                    Log.W($"{token} is not a valid profession name.");
                    continue;
                }

                if (prestige)
                {
                    Log.W($"Cannot prestige custom skill profession {customProfession.StringId}.");
                    continue;
                }

                if (player.HasProfession(customProfession))
                {
                    Log.W(
                        $"Farmer {player.Name} already has the {customProfession.StringId} profession.");
                    continue;
                }

                professionsToAdd.Add(customProfession.Id);
                Log.I($"Added the {customProfession.StringId} profession to {player.Name}.");
            }
        }

        LevelUpMenu levelUpMenu = new();
        foreach (var pid in professionsToAdd.Distinct().Except(player.professions))
        {
            if (player.professions.AddOrReplace(pid))
            {
                levelUpMenu.getImmediateProfessionPerk(pid);
            }
        }

        LevelUpMenu.RevalidateHealth(player);
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
