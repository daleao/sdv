namespace DaLion.Overhaul.Modules.Tweex.Commands;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Overhaul.Modules.Tweex.Extensions;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;
using StardewValley.TerrainFeatures;

#endregion using directives

[UsedImplicitly]
internal sealed class SetAgeCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="SetAgeCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetAgeCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "set_age", "age" };

    /// <inheritdoc />
    public override string Documentation =>
        "Set the age of the nearest specified object or tree to the desired number of days. You can also use the value `clear` to delete the respective mod data.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        if (args.Length is < 2 or > 3)
        {
            Log.W("You must specify a target type and age value." + this.GetUsage());
            return;
        }

        var all = args.Any(a => a is "-a" or "--all");
        if (all)
        {
            args = args.Except(new[] { "-a", "--all" }).ToArray();
        }

        var clear = false;
        if (args[1].ToLowerInvariant() is "clear" or "null")
        {
            clear = true;
        }
        else if (!int.TryParse(args[1], out _))
        {
            Log.W($"{args[1]} is not a valid age value. Please specify a valid number of days.");
            return;
        }

        switch (args[0].ToLowerInvariant())
        {
            case "tree":
            {
                if (all)
                {
                    for (var i = 0; i < Game1.locations.Count; i++)
                    {
                        var location = Game1.locations[i];
                        foreach (var feature in location.terrainFeatures.Values)
                        {
                            if (feature is Tree)
                            {
                                feature.Write(DataKeys.Age, clear ? null : args[1]);
                            }
                        }
                    }

                    Log.I(clear ? "Cleared all tree age data." : $"Set all tree age data to {args[1]} days.");
                    break;
                }

                var nearest = Game1.player.GetClosestTerrainFeature<Tree>(out _);
                if (nearest is null)
                {
                    Log.W("There are no trees nearby.");
                    return;
                }

                nearest.Write(DataKeys.Age, clear ? null : args[1]);
                Log.I(clear
                    ? $"Cleared {nearest.NameFromType()}'s age data"
                    : $"Set {nearest.NameFromType()}'s age data to {args[1]} days.");
                break;
            }

            case "bee":
            case "hive":
            case "beehive":
            case "beehouse":
            {
                if (all)
                {
                    for (var i = 0; i < Game1.locations.Count; i++)
                    {
                        var location = Game1.locations[i];
                        foreach (var @object in location.Objects.Values)
                        {
                            if (@object.Name != "Bee House")
                            {
                                continue;
                            }

                            @object.Write(DataKeys.Age, clear ? null : args[1]);
                        }
                    }

                    Log.I(clear ? "Cleared all bee house age data." : $"Set all bee house age data to {args[1]} days.");
                    break;
                }

                var nearest =
                    Game1.player.GetClosestObject<SObject>(
                        out _,
                        predicate: o => o.bigCraftable.Value && o.Name == "Bee House");
                if (nearest is null)
                {
                    Log.W("There are no bee houses nearby.");
                    return;
                }

                nearest.Write(DataKeys.Age, clear ? null : args[1]);
                Log.I(clear
                    ? "Cleared all Bee House age data."
                    : $"Set all Bee House age data to {args[1]} days.");
                break;
            }

            case "mushroom":
            case "shroom":
            case "box":
            case "mushroombox":
            case "shroombox":
            {
                if (all)
                {
                    for (var i = 0; i < Game1.locations.Count; i++)
                    {
                        var location = Game1.locations[i];
                        foreach (var @object in location.Objects.Values)
                        {
                            if (@object.Name != "Mushroom Box")
                            {
                                continue;
                            }

                            @object.Write(DataKeys.Age, clear ? null : args[1]);
                        }
                    }

                    Log.I(clear
                        ? "Cleared all mushroom box age data."
                        : $"Set all mushroom box age data to {args[1]} days.");
                    break;
                }

                var nearest =
                    Game1.player.GetClosestObject<SObject>(out _, predicate: o =>
                        o.bigCraftable.Value && o.Name == "Mushroom Box");
                if (nearest is null)
                {
                    Log.W("There are no mushroom boxes nearby.");
                    return;
                }

                nearest.Write(DataKeys.Age, clear ? null : args[1]);
                Log.I(clear ? "Cleared Mushroom Box's age data." : $"Set Mushroom Box's age data to {args[1]} days.");
                break;
            }
        }
    }

    private string GetUsage()
    {
        var result =
            new StringBuilder(
                $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} [--all / -a] <target type> <age>");
        result.Append("\n\nParameters:");
        result.Append("\n\t- <target type>\t- one of 'tree', 'beehive' or 'mushroombox'");
        result.Append("\n\nOptional flags:");
        result.Append(
            "\n\t--all, -a\t- set the age of all instances of the specified type, instead of just the nearest one.");
        result.Append("\n\nExamples:");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} hive 112");
        result.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} -a tree 224");
        return result.ToString();
    }
}
