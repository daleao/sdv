namespace DaLion.Overhaul.Modules.Professions.Commands;

#region using directives

using DaLion.Shared.Commands;
using DaLion.Shared.Extensions.Stardew;

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
        if (args.Length == 0)
        {
            Log.W("You must specify either 'friendship', 'happiness' or 'both'.");
            return;
        }

        var both = args.Length == 0 || string.Equals(args[0], "both", StringComparison.InvariantCultureIgnoreCase);
        var count = 0;
        foreach (var animal in Game1.getFarm().getAllFarmAnimals())
        {
            if (!animal.IsOwnedBy(Game1.player))
            {
                continue;
            }

            if (both)
            {
                animal.friendshipTowardFarmer.Value = 1000;
                animal.happiness.Value = 255;
            }
            else
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "friendship" or "friendly":
                        animal.friendshipTowardFarmer.Value = 1000;
                        break;
                    case "happiness" or "happy":
                        animal.happiness.Value = 255;
                        break;
                }
            }

            count++;
        }

        if (count > 0)
        {
            if (both)
            {
                Log.I($"Maxed both the friendship and happiness of {count} animals");
            }
            else
            {
                switch (args[0].ToLowerInvariant())
                {
                    case "friendship" or "friendly":
                        Log.I($"Maxed the friendship of {count} animals");
                        break;
                    case "happiness" or "happy":
                        Log.I($"Maxed the happiness of {count} animals");
                        break;
                }
            }
        }
        else
        {
            Log.W("You don't own any animals.");
        }
    }
}
