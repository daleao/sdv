namespace DaLion.Stardew.Tweex.Commands;

#region using directives

using Common;
using Common.Commands;
using Common.Data;
using Extensions;
using JetBrains.Annotations;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Linq;
using SObject = StardewValley.Object;

#endregion using directives

[UsedImplicitly]
internal sealed class SetAgeCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal SetAgeCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "age";

    /// <inheritdoc />
    public override string Documentation => "Age the nearest object of the specified type.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        if (args.Length is <2 or >3)
        {
            Log.W("You must specify an object type and age value." + GetUsage());
            return;
        }

        var all = args.Any(a => a is "-a" or "--all");
        if (all) args = args.Except(new[] { "-a", "--all" }).ToArray();

        if (!int.TryParse(args[1], out var newAge))
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
                        ModDataIO.WriteTo(tree, "Age", args[1]);
                    Log.I($"Set all tree age data to {args[1]} days.");

                    break;
                }

                var nearest = Game1.player.GetClosestTree(out _);
                if (nearest is null)
                {
                    Log.W("There are no trees nearby.");
                    return;
                }

                ModDataIO.WriteTo(nearest, "Age", args[1]);
                Log.I($"Set {nearest.NameFromType()}'s age data to {args[1]} days.");
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
                        ModDataIO.WriteTo(hive, "Age", args[1]);
                    Log.I($"Set all bee house age data to {args[1]} days.");

                    break;
                }

                var nearest = Game1.player.GetClosestBigCraftable(out _, predicate: o => o.Name == "Bee House");
                if (nearest is null)
                {
                    Log.W("There are no bee houses nearby.");
                    return;
                }

                ModDataIO.WriteTo(nearest, "Age", args[1]);
                Log.I($"Set Bee House's age data to {args[1]} days.");
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
                        ModDataIO.WriteTo(box, "Age", args[1]);
                    Log.I($"Set all mushroom box age data to {args[1]} days.");

                    break;
                }

                var nearest = Game1.player.GetClosestBigCraftable(out _, predicate: o => o.Name == "Mushroom Box");
                if (nearest is null)
                {
                    Log.W("There are no mushroom boxes nearby.");
                    return;
                }

                ModDataIO.WriteTo(nearest, "Age", args[1]);
                Log.I($"Set Mushroom Box's age data to {args[1]} days.");
                break;
            }
        }
    }

    private string GetUsage()
    {
        var result = $"\n\nUsage: {Handler.EntryCommand} {Trigger} [--all / -a] <target type> <age>";
        result += "\n\nParameters:";
        result += "\n\t- <target type>\t- one of 'tree', 'beehive' or 'mushroombox'";
        result += "\n\nOptional flags:";
        result +=
            "\n\t--all, -a\t- set the age of all instances of the specified type, instead of just the nearest one.";
        result += "\n\nExamples:";
        result += $"\n\t- {Handler.EntryCommand} {Trigger} hive 112";
        result += $"\n\t- {Handler.EntryCommand} {Trigger} -a tree 224";
        return result;
    }
}