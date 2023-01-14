namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class MaxAnimalDispositionsCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="MaxAnimalDispositionsCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal MaxAnimalDispositionsCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "animals" };

    /// <inheritdoc />
    public override string Documentation =>
        $"Maxes-out the friendship and/or happiness of all owned animals. Relevant for {Profession.Breeder.Name} and {Profession.Producer.Name}.";

    /// <inheritdoc />
    public override void Callback(string trigger, string[] args)
    {
        var animals = Game1.getFarm().getAllFarmAnimals().Where(a =>
            a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer).ToList();
        var count = animals.Count;
        if (count == 0)
        {
            Log.W("You don't own any animals.");
            return;
        }

        foreach (var animal in animals)
        {
            if (args.Length == 0 || args[0].ToLowerInvariant() == "both")
            {
                animal.friendshipTowardFarmer.Value = 1000;
                animal.happiness.Value = 255;
                Log.I($"Maxed the friendship and happiness of {count} animals");
            }
            else
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "friendship" or "friendly":
                        animal.friendshipTowardFarmer.Value = 1000;
                        Log.I($"Maxed the friendship of {count} animals");
                        break;
                    case "happiness" or "happy":
                        animal.happiness.Value = 255;
                        Log.I($"Maxed the happiness of {count} animals");
                        break;
                }
            }
        }
    }
}
