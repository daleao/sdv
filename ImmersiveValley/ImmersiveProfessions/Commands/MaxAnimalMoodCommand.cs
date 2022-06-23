namespace DaLion.Stardew.Professions.Commands;

#region using directives

using System.Linq;
using StardewModdingAPI;
using StardewValley;

using Common;
using Common.Commands;

#endregion using directives

internal class MaxAnimalMoodCommand : ICommand
{
    /// <inheritdoc />
    public string Trigger => "max_animal_mood";

    /// <inheritdoc />
    public string Documentation => "Max-out the mood of all owned animals.";

    /// <inheritdoc />
    public void Callback(string[] args)
    {
        var animals = Game1.getFarm().getAllFarmAnimals().Where(a =>
            a.ownerID.Value == Game1.player.UniqueMultiplayerID || !Context.IsMultiplayer).ToList();
        var count = animals.Count;

        if (count <= 0)
        {
            Log.W("You don't own any animals.");
            return;
        }

        foreach (var animal in animals) animal.happiness.Value = 255;
        Log.I($"Maxed the mood of {count} animals");
    }
}