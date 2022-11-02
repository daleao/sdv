namespace DaLion.Redux.Framework.Professions.Commands;

#region using directives

using System.Linq;
using DaLion.Shared.Commands;

#endregion using directives

[UsedImplicitly]
internal sealed class MaxAnimalMoodCommand : ConsoleCommand
{
    /// <summary>Initializes a new instance of the <see cref="MaxAnimalMoodCommand"/> class.</summary>
    /// <param name="handler">The <see cref="CommandHandler"/> instance that handles this command.</param>
    internal MaxAnimalMoodCommand(CommandHandler handler)
        : base(handler)
    {
    }

    /// <inheritdoc />
    public override string[] Triggers { get; } = { "max_animal_mood", "max_mood", "happy_animals", "happy" };

    /// <inheritdoc />
    public override string Documentation =>
        $"Max-out the mood of all owned animals. Relevant for {Profession.Producer}s.";

    /// <inheritdoc />
    public override void Callback(string[] args)
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
            animal.happiness.Value = 255;
        }

        Log.I($"Maxed the mood of {count} animals");
    }
}
