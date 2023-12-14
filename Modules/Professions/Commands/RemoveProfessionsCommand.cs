namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.SMAPI;
using StardewValley.Menus;

#endregion using directives

[UsedImplicitly]
internal sealed class RemoveProfessionsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="RemoveProfessionsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal RemoveProfessionsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "remove_professions", "remove_profs", "remove", "clear" };

    /// <inheritdoc />
    public override string Documentation =>
        "Remove the specified professions from the player. Does not affect skill levels." + this.GetUsage();

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (trigger == "clear")
        {
            var shouldInvalidate = Game1.player.professions.Intersect(VanillaProfession.GetRange(true)).Any();
            Game1.player.professions.Clear();
            LevelUpMenu.RevalidateHealth(Game1.player);
            if (shouldInvalidate)
            {
                ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
            }

            Log.I($"Cleared all professions from {Game1.player.Name}.");
            return;
        }

        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("You must specify at least one profession." + this.GetUsage());
            return;
        }

        List<int> professionsToRemove = new();
        for (var i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], "all", StringComparison.InvariantCultureIgnoreCase))
            {
                var shouldInvalidate = Game1.player.professions.Intersect(VanillaProfession.GetRange(true)).Any();
                Game1.player.professions.Clear();
                LevelUpMenu.RevalidateHealth(Game1.player);
                if (shouldInvalidate)
                {
                    ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
                }

                Log.I($"Removed all professions from {Game1.player.Name}.");
                break;
            }

            if (string.Equals(args[i], "rogue", StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(args[i], "unknown", StringComparison.InvariantCultureIgnoreCase))
            {
                var range = Game1.player.professions
                    .Where(pid =>
                        !VanillaProfession.TryFromValue(pid, out _) &&
                        CustomProfession.List.All(p => pid != p.Id))
                    .ToArray();

                professionsToRemove.AddRange(range);
                Log.I($"Removed unknown professions from {Game1.player.Name}.");
            }
            else if (VanillaProfession.TryFromName(args[i], true, out var profession) ||
                     VanillaProfession.TryFromLocalizedName(args[i], true, out profession))
            {
                professionsToRemove.Add(profession.Id);
                professionsToRemove.Add(profession.Id + 100);
                Log.I($"Removed {profession.StringId} profession from {Game1.player.Name}.");
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

                professionsToRemove.Add(customProfession.Id);
                Log.I($"Removed {customProfession.StringId} profession from {Game1.player.Name}.");
            }
        }

        foreach (var pid in professionsToRemove.Distinct())
        {
            GameLocation.RemoveProfession(pid);
        }

        LevelUpMenu.RevalidateHealth(Game1.player);
        if (professionsToRemove.Intersect(VanillaProfession.GetRange(true)).Any())
        {
            ModHelper.GameContent.InvalidateCacheAndLocalized("LooseSprites/Cursors");
        }
    }

    private string GetUsage()
    {
        var result =
            new StringBuilder(
                $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} [--prestige] <profession1> <profession2> ... <professionN>");
        result.Append("\n\nParameters:");
        result.Append(
            "\n\t- <profession>\t- a valid profession name, `all` or `unknown`. Use `unknown` to remove rogue professions from uninstalled custom skill mods.");
        result.Append("\n\nExamples:");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} artisan brute");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} -p all");
        return result.ToString();
    }
}
