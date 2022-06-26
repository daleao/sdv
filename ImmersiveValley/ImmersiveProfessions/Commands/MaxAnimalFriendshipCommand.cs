namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System.Linq;
using JetBrains.Annotations;
using StardewModdingAPI;
using StardewValley;

using Common;
using Common.Commands;
using Framework;

#endregion using directives

[UsedImplicitly]
internal sealed class MaxAnimalFriendshipCommand : ConsoleCommand
{
    /// <summary>Construct an instance.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal MaxAnimalFriendshipCommand(CommandHandler handler)
        : base(handler) { }

    /// <inheritdoc />
    public override string Trigger => "max_animal_friendship";

    /// <inheritdoc />
    public override string Documentation => $"Max-out the friendship of all owned animals. Relevant for {Profession.Breeder}s";

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