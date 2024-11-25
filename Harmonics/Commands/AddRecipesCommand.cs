namespace DaLion.Harmonics.Commands;

#region using directives

using System.Linq;
using System.Text;
using DaLion.Shared.Commands;
using StardewValley;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="AddRecipesCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
internal sealed class AddRecipesCommand(CommandHandler handler) : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["recipe"];

    /// <inheritdoc />
    public override string Documentation => "Adds the specified Gemstone Ring recipe to the player's crafting menu.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
        {
            Log.W("No recipe was specified.");
            return false;
        }

        var player = Game1.player;
        List<CraftingRecipe> recipes = [];
        while (args.Length > 0)
        {
            if (args[0] == "level")
            {
                if (player.CombatLevel >= 2)
                {
                    if (player.craftingRecipes.TryAdd("Amethyst Ring", 0))
                    {
                        Log.I($"Added Amethyst Ring recipe to {player.Name}.");
                    }

                    if (player.craftingRecipes.TryAdd("Topaz Ring", 0))
                    {
                        Log.I($"Added Topaz Ring recipe to {player.Name}.");
                    }
                }

                if (player.CombatLevel >= 4)
                {
                    if (player.craftingRecipes.TryAdd("Aquamarine Ring", 0))
                    {
                        Log.I($"Added Aquamarine Ring recipe to {player.Name}.");
                    }

                    if (player.craftingRecipes.TryAdd("Jade Ring", 0))
                    {
                        Log.I($"Added Jade Ring recipe to {player.Name}.");
                    }
                }

                if (player.CombatLevel >= 6)
                {
                    if (player.craftingRecipes.TryAdd("Ruby Ring", 0))
                    {
                        Log.I($"Added Ruby Ring recipe to {player.Name}.");
                    }

                    if (player.craftingRecipes.TryAdd("Emerald Ring", 0))
                    {
                        Log.I($"Added Emerald Ring recipe to {player.Name}.");
                    }
                }

                if (player.CombatLevel >= 7)
                {
                    if (player.craftingRecipes.TryAdd("Garnet Ring", 0))
                    {
                        Log.I($"Added Garnet Ring recipe to {player.Name}.");
                    }
                }

                return true;
            }

            switch (args[0].ToLowerInvariant())
            {
                case "topaz":
                    if (player.craftingRecipes.TryAdd("Topaz Ring", 0))
                    {
                        Log.I($"Added Topaz Ring recipe to {player.Name}.");
                    }

                    break;
                case "amethyst":
                    if (player.craftingRecipes.TryAdd("Amethyst Ring", 0))
                    {
                        Log.I($"Added Amethyst Ring recipe to {player.Name}.");
                    }

                    break;
                case "aquamarine":
                    if (player.craftingRecipes.TryAdd("Aquamarine Ring", 0))
                    {
                        Log.I($"Added Aquamarine Ring recipe to {player.Name}.");
                    }

                    break;
                case "jade":
                    if (player.craftingRecipes.TryAdd("Jade Ring", 0))
                    {
                        Log.I($"Added Jade Ring recipe to {player.Name}.");
                    }

                    break;
                case "ruby":
                    if (player.craftingRecipes.TryAdd("Ruby Ring", 0))
                    {
                        Log.I($"Added Ruby Ring recipe to {player.Name}.");
                    }

                    break;
                case "emerald":
                    if (player.craftingRecipes.TryAdd("Emerald Ring", 0))
                    {
                        Log.I($"Added Emerald Ring recipe to {player.Name}.");
                    }

                    break;
                case "garnet":
                    if (player.craftingRecipes.TryAdd("Garnet Ring", 0))
                    {
                        Log.I($"Added Garnet Ring recipe to {player.Name}.");
                    }

                    break;
                default:
                    Log.E($"Invalid ring type {args[0]}");
                    return false;
            }

            args = args.Skip(1).ToArray();
        }

        return true;
    }

    /// <inheritdoc />
    protected override string GetUsage()
    {
        var sb = new StringBuilder($"\n\nUsage: {this.Handler.EntryCommand} {this.Triggers[0]} <gemstone>");
        sb.Append("\n\nParameters:");
        sb.Append("\n\t- <gemstone>: any of the 7 gemstones, or \"level\" to add all gemstones for the player's current Combat level");
        sb.Append("\n\nExample:");
        sb.Append($"\n\t- {this.Handler.EntryCommand} {this.Triggers[0]} ruby emerald garnet");
        return sb.ToString();
    }
}
