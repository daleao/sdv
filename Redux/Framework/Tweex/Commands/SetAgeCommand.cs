namespace DaLion.Redux.Framework.Tweex.Commands;

#region using directives

using System.Linq;
using DaLion.Redux.Framework.Tweex.Extensions;
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
        "Set the age of the nearest specified object or tree to the desired value. You can also use the value `clear` to delete the respective mod data.";

    /// <inheritdoc />
    public override void Callback(string[] args)
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
                    foreach (var tree in Game1.locations.SelectMany(l => l.terrainFeatures.Values.OfType<Tree>()))
                    {
                        tree.Write(DataFields.Age, clear ? null : args[1]);
                    }

                    Log.I(clear ? "Cleared all tree age data." : $"Set all tree age data to {args[1]} days.");
                    break;
                }

                var nearest = Game1.player.GetClosestTerrainFeature<Tree>();
                if (nearest is null)
                {
                    Log.W("There are no trees nearby.");
                    return;
                }

                nearest.Write(DataFields.Age, clear ? null : args[1]);
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
                    foreach (var hive in Game1.locations.SelectMany(l =>
                                 l.objects.Values.Where(o => o.Name == "Bee House")))
                    {
                        hive.Write(DataFields.Age, clear ? null : args[1]);
                    }

                    Log.I(clear ? "Cleared all bee house age data." : $"Set all bee house age data to {args[1]} days.");
                    break;
                }

                var nearest =
                    Game1.player.GetClosestObject<SObject>(
                        predicate: o => o.bigCraftable.Value && o.Name == "Bee House");
                if (nearest is null)
                {
                    Log.W("There are no bee houses nearby.");
                    return;
                }

                nearest.Write(DataFields.Age, clear ? null : args[1]);
                Log.I(clear ? "Cleared Bee House's age data." : $"Set Bee House's age data to {args[1]} days.");
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
                    foreach (var box in Game1.locations.SelectMany(l =>
                                 l.objects.Values.Where(o => o.Name == "Mushroom Box")))
                    {
                        box.Write(DataFields.Age, clear ? null : args[1]);
                    }

                    Log.I(clear
                        ? "Cleared all mushroom box age data."
                        : $"Set all mushroom box age data to {args[1]} days.");
                    break;
                }

                var nearest =
                    Game1.player.GetClosestObject<SObject>(predicate: o =>
                        o.bigCraftable.Value && o.Name == "Mushroom Box");
                if (nearest is null)
                {
                    Log.W("There are no mushroom boxes nearby.");
                    return;
                }

                nearest.Write(DataFields.Age, clear ? null : args[1]);
                Log.I(clear ? "Cleared Mushroom Box's age data." : $"Set Mushroom Box's age data to {args[1]} days.");
                break;
            }
        }
    }

    private string GetUsage()
    {
        var result = $"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers.First()} [--all / -a] <target type> <age>";
        result += "\n\nParameters:";
        result += "\n\t- <target type>\t- one of 'tree', 'beehive' or 'mushroombox'";
        result += "\n\nOptional flags:";
        result +=
            "\n\t--all, -a\t- set the age of all instances of the specified type, instead of just the nearest one.";
        result += "\n\nExamples:";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} hive 112";
        result += $"\n\t- {this.Handler.EntryCommand} {this.Triggers.First()} -a tree 224";
        return result;
    }
}
