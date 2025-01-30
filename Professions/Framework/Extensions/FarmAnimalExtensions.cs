namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Shared.Extensions.Stardew;

#endregion

/// <summary>Extensions for the <see cref="FarmAnimal"/> class.</summary>
internal static class FarmAnimalExtensions
{
    /// <summary>Determines whether the owner of the <paramref name="animal"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <param name="profession">An <see cref="IProfession"/>..</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="animal"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesOwnerHaveProfession(this FarmAnimal animal, IProfession profession, bool prestiged = false)
    {
        return animal.GetOwner().HasProfession(profession, prestiged);
    }

    /// <summary>Determines whether the owner of the <paramref name="animal"/>---or any <see cref="Farmer"/> instance in the game session, if allowed by the module's settings---has the specified <paramref name="profession"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <param name="profession">An <see cref="IProfession"/>..</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="animal"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    internal static bool DoesOwnerHaveProfessionOrLax(this FarmAnimal animal, IProfession profession, bool prestiged = false)
    {
        return animal.GetOwner().HasProfessionOrLax(profession, prestiged);
    }

    /// <summary>Adjusts the price of the <paramref name="animal"/> for <see cref="Profession.Breeder"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <returns>The adjusted friendship value.</returns>
    internal static double GetBreederAdjustedFriendship(this FarmAnimal animal)
    {
        return animal.DoesOwnerHaveProfessionOrLax(Profession.Breeder, true)
            ? Config.BreederFriendlyAnimalMultiplier
            : Config.BreederFriendlyAnimalMultiplier * animal.friendshipTowardFarmer.Value / 1000;
    }
}
