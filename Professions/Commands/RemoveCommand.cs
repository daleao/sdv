namespace DaLion.Professions.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
using DaLion.Shared.Extensions.Stardew;
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

    internal static Dictionary<int, Queue<int>> ProfessionsToRemovePerScreen { get; } = [];

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
        int? screenId = 0;
        if (farmerIndex > 1)
        {
            if (!Context.IsSplitScreen)
            {
                Log.W("Can't assign professions to co-op players in a non-splitscreen session.");
                return false;
            }

            var peerIndex = farmerIndex - 2; // subtract 1 for host player and 1 for zero-index
            var onlinePlayers = ModHelper.Multiplayer.GetConnectedPlayers().ToList();
            if (peerIndex >= onlinePlayers.Count)
            {
                Log.W($"Insufficient online players for setting specified player \"{farmerIndex}\".");
                return false;
            }

            var multiplayerId = onlinePlayers[peerIndex].PlayerID;
            player = Game1.GetPlayer(multiplayerId, onlyOnline: true);
            if (player is null)
            {
                Log.W($"Failed to get online player number {farmerIndex}.");
                return false;
            }

            screenId = player.GetScreenId(ModHelper.Multiplayer);
            if (screenId is null)
            {
                Log.W($"Failed to get {player.Name}'s splitscreen ID.");
                return false;
            }
        }

        if (args[0].ToLower() is "mastery" or "masteries")
        {
            args = [.. args.Skip(1)];
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

        HashSet<(int Id, string Name)> professionsToRemove = [];
        foreach (var token in tokens)
        {
            if (string.Equals(token, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var shouldInvalidate = player.professions.Intersect(Profession.GetRange(true)).Any();
                player.professions.Clear();
                LevelUpMenu.RevalidateHealth(player);
                if (shouldInvalidate)
                {
                    ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
                }

                break;
            }

            if (string.Equals(token, "rogue", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(token, "unknown", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = player.professions
                    .Where(pid =>
                        !Profession.TryFromValue(pid, out _) && !Profession.TryFromValue(pid + 100, out _) &&
                        CustomProfession.List.All(p => pid != p.Id && pid != p.Id + 100))
                    .ToHashSet();

                professionsToRemove.UnionWith(range.Select(i => (i, "Unknown")));
            }
            else if (Profession.TryFromName(token, true, out var profession) ||
                     Profession.TryFromLocalizedName(token, true, out profession) ||
                     (int.TryParse(token, out var id) && Profession.TryFromValue(id, out profession)))
            {
                professionsToRemove.Add((profession.Id, profession.Name));
            }
            else
            {
                var customProfession = CustomProfession.List.FirstOrDefault(p =>
                    string.Equals(token, p.StringId.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(token, p.Title.TrimAll(), StringComparison.InvariantCultureIgnoreCase) ||
                    (int.TryParse(token, out id) && id == p.Id));
                if (customProfession is null)
                {
                    Log.W($"Ignoring unknown profession {token}.");
                    continue;
                }

                professionsToRemove.Add((customProfession.Id, customProfession.StringId));
            }
        }

        if (player.IsLocalPlayer)
        {
            foreach (var (pid, pname) in professionsToRemove)
            {
                if (!player.professions.Remove(pid))
                {
                    if (player.professions.Remove(pid + 100))
                    {
                        Log.I($"{Game1.player.Name} does not have profession {pname}, but does have the prestige.");

                        GameLocation.RemoveProfession(pid + 100);
                        Log.I($"Removed prestiged {pname} (Prestige) profession from {Game1.player.Name}.");
                        continue;
                    }

                    Log.I($"{Game1.player.Name} does not have profession {pname}.");
                    continue;
                }

                GameLocation.RemoveProfession(pid);
                Log.I($"Removed {pname} profession from {Game1.player.Name}.");
                if (player.professions.Remove(pid + 100))
                {
                    GameLocation.RemoveProfession(pid + 100);
                    Log.I($"Removed prestiged {pname} (Prestiged) profession from {Game1.player.Name}.");
                    continue;
                }
            }

            return true;
        }

        ProfessionsToRemovePerScreen[screenId.Value] = new(professionsToRemove.Select(p => p.Id));
        return true;
    }

    internal static void RemoveProfessionsStatic(Queue<int> professionsToRemove, Farmer player)
    {
        while (professionsToRemove.TryDequeue(out var pid))
        {
            if (player.professions.Remove(pid))
            {
                GameLocation.RemoveProfession(pid);
                Log.I($"Removed profession ID {pid} from {player.Name}.");

                if (pid.IsIn(Profession.GetRange(true)))
                {
                    ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
                }
            }
        }

        LevelUpMenu.RevalidateHealth(player);
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
