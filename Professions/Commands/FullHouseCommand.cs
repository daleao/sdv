namespace DaLion.Professions.Commands;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Commands;
using DaLion.Shared.Extensions;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="FullHouseCommand"/> class.</summary>
/// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
[UsedImplicitly]
[Debug]
internal sealed class FullHouseCommand(CommandHandler handler)
    : ConsoleCommand(handler)
{
    /// <inheritdoc />
    public override string[] Triggers { get; } = ["full_house", "fullhouse", "full"];

    /// <inheritdoc />
    public override string Documentation => "Fills the nearest Barn or Coop with either mature Cows or Chickens, up to capacity.";

    /// <inheritdoc />
    public override bool CallbackImpl(string trigger, string[] args)
    {
        if (args.Length > 1)
        {
            Log.W("Additional arguments will be ignored.");
        }

        var nearest = Game1.player.GetClosestBuilding(predicate: b =>
            b.GetIndoors() is AnimalHouse house && house.Name.ContainsAnyOf("Barn", "Coop") &&
            b.IsOwnedBy(Game1.player) && !b.isUnderConstruction() && b.TileDistanceToPlayer() < 10);
        if (nearest is null)
        {
            Log.W("There are no barns or coops nearby.");
            return true;
        }

        var house = (nearest.GetIndoors() as AnimalHouse)!;
        while (!house.isFull())
        {
            var animal = house.Name.Contains("Barn")
                ? new FarmAnimal(
                    Game1.random.Choose("White Cow", "Brown Cow", "Goat", "Sheep"),
                    Game1.Multiplayer.getNewID(),
                    Game1.player.UniqueMultiplayerID)
                : new FarmAnimal(
                    Game1.random.Choose("White Chicken", "Brown Chicken", "Duck", "Rabbit"),
                    Game1.Multiplayer.getNewID(),
                    Game1.player.UniqueMultiplayerID);
            animal.growFully();
            house.adoptAnimal(animal);
        }

        Log.I($"The nearby {house.Name} has been filled.");
        return true;
    }
}
