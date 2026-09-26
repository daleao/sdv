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
using DaLion.Shared.Extensions.Stardew;
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
    public override string[] Triggers { get; } = ["add"];

    /// <inheritdoc />
    public override string Documentation =>
        "Add the specified professions to the player without affecting skill levels. Can also be used to add masteries to the specified skills using the keyword \"mastery\".";

    internal static Dictionary<int, Queue<int>> ProfessionsToAddPerScreen { get; } = [];

    internal static Dictionary<int, Queue<string>> RecipesToAddPerScreen { get; } = [];

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

        HashSet<string> recipesToAdd = [];
        if (args.Length == 1)
        {
            var recipeToAdd = false;
            if (args[0].ToLowerInvariant() is "flag" or "surveyflag")
            {
                recipesToAdd.Add("Survey Flag");
                recipeToAdd = true;
            }
            else if (args[0].ToLowerInvariant() is "flute" or "slimeflute")
            {
                recipesToAdd.Add("Slime Flute");
                recipeToAdd = true;
            }
            else if (args.Length == 1 && (args[0].ToLowerInvariant() is "brushes" or "paint" or "paintbrushes"))
            {
                recipesToAdd.Add("Green Paintbrush");
                recipesToAdd.Add("Blue Paintbrush");
                recipesToAdd.Add("Red Paintbrush");
                recipesToAdd.Add("Purple Paintbrush");
                recipesToAdd.Add("Prismatic Paintbrush");
                recipeToAdd = true;
            }

            if (recipeToAdd)
            {
                if (player.IsLocalPlayer)
                {
                    foreach (var recipe in recipesToAdd)
                    {
                        if (player.craftingRecipes.TryAdd(recipe, 0))
                        {
                            Log.I($"Added {recipe} recipe to {player.Name}.");
                        }
                    }

                    return true;
                }

                RecipesToAddPerScreen[screenId.Value] = new(recipesToAdd);
                return true;
            }
        }

        if (tokens[0].ToLower() is "mastery" or "masteries")
        {
            tokens = [.. tokens.Skip(1)];
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
                    player.stats.Set(StatKeys.MasteryLevelsSpent, 5);
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
                    player.stats.Set(StatKeys.MasteryLevelsSpent, player.stats.Get(StatKeys.MasteryLevelsSpent) + 1);
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
            tokens = [.. tokens.Except(prestigeArgs)];
        }

        HashSet<(int Id, string Name)> professionsToAdd = [];
        foreach (var token in tokens)
        {
            if (string.Equals(token, "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Profession.GetRange().ToArray();
                if (prestige)
                {
                    range = [.. range, .. Profession.GetRange(true)];
                }

                range = [.. range, .. CustomProfession.List.Select(p => p.Id)];
                professionsToAdd.UnionWith(range.Select(i => (i, string.Empty)));
                Log.I(
                    $"Adding all {(prestige ? "prestiged " : string.Empty)}professions to {player.Name}.");
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

                professionsToAdd.Add((profession.Id, profession.Name));
                if (prestige)
                {
                    professionsToAdd.Add((profession.Id + 100, profession.Name + " (Prestiged)"));
                }
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

                professionsToAdd.Add((customProfession.Id, customProfession.StringId));
            }
        }

        if (player.IsLocalPlayer)
        {
            LevelUpMenu levelUpMenu = new();
            foreach (var (pid, pname) in professionsToAdd)
            {
                if (player.professions.AddOrReplace(pid))
                {
                    levelUpMenu.getImmediateProfessionPerk(pid);
                    Log.I($"Added the {pname} profession to {player.Name}.");

                    if (pid.IsIn(Profession.GetRange(true)))
                    {
                        ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
                    }
                }

            }

            LevelUpMenu.RevalidateHealth(player);
            return true;
        }

        ProfessionsToAddPerScreen[screenId.Value] = new(professionsToAdd.Select(p => p.Id));
        return true;
    }

    internal static void AddProfessionsStatic(Queue<int> professionsToAdd, Farmer player)
    {
        LevelUpMenu levelUpMenu = new();
        while (professionsToAdd.TryDequeue(out var pid))
        {
            if (player.professions.AddOrReplace(pid))
            {
                levelUpMenu.getImmediateProfessionPerk(pid);
                Log.I($"Added profession ID {pid} to {player.Name}.");

                if (pid.IsIn(Profession.GetRange(true)))
                {
                    ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
                }
            }
        }

        LevelUpMenu.RevalidateHealth(player);
    }

    internal static void AddRecipesStatic(Queue<string> recipesToAdd, Farmer player)
    {
        while (recipesToAdd.TryDequeue(out var recipe))
        {
            if (player.craftingRecipes.TryAdd(recipe, 0))
            {
                Log.I($"Added {recipe} recipe to {player.Name}.");
            }
        }
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
