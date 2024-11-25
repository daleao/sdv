namespace DaLion.Shared.Extensions.Stardew;

/// <summary>Extensions for the <see cref="FarmAnimal"/> class.</summary>
public static class FarmAnimalExtensions
{
    /// <summary>Gets the <see cref="Farmer"/> instance who owns this <paramref name="animal"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <returns>The <see cref="Farmer"/> instance who purchased or owned the parent of the <paramref name="animal"/>, or the host of the game session if not found.</returns>
    public static Farmer GetOwner(this FarmAnimal animal)
    {
        return Game1.GetPlayer(animal.ownerID.Value) ?? Game1.MasterPlayer;
    }

    /// <summary>Checks whether the <paramref name="animal"/> is owned by the specified <see cref="Farmer"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <param name="farmer">The <see cref="Farmer"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="animal"/>'s owner ID  value is equal to the unique ID of the <paramref name="farmer"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsOwnedBy(this FarmAnimal animal, Farmer farmer)
    {
        return animal.ownerID.Value == farmer.UniqueMultiplayerID;
    }
}
