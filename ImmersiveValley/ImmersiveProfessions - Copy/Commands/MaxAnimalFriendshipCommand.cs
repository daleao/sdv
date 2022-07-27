namespace DaLion.Stardew.Professions.Commands;

#region using directives

using Common;
using Common.Commands;
using Framework;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;
using System.Linq;

#endregion using directives

[UsedImplicitly]
internal sealed class MaxAnimalFriendshipCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal MaxAnimalFriendshipCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "max_animal_friendship", "friendly_animals", "friendly" };

    /// <inheritdoc />
    public override string Documentation =>
        $"Max-out the friendship of all owned animals. Relevant for {Profession.Breeder.Name}s.";

    /// <inheritdoc />
    public override void Callback(string[] args)
    {
        var animals = Game1.getFarm().getAllFarmAnimals().Where(a =>
            a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer).ToList();
        var count = animals.Count;
        if (count <= 0)
        {
            Log.W("You don't own any animals.");
            return;
        }

        foreach (var animal in animals) animal.friendshipTowardFarmer.Value = 1000;
        Log.I($"Maxed the friendship of {count} animals");
    }
}