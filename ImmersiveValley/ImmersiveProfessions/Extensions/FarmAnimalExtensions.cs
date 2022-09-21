namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System;
using DaLion.Common.Extensions.Stardew;
using DaLion.Stardew.Professions.Framework;
using StardewValley.Buildings;

#endregion

/// <summary>Extensions for the <see cref="FarmAnimal"/> class.</summary>
public static class FarmAnimalExtensions
{
    /// <summary>Determines whether the owner of the <paramref name="animal"/> has the specified <paramref name="profession"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <param name="profession">An <see cref="IProfession"/>..</param>
    /// <param name="prestiged">Whether to check for the prestiged variant.</param>
    /// <returns><see langword="true"/> if the <see cref="Farmer"/> who owns the <paramref name="animal"/> has the <paramref name="profession"/>, otherwise <see langword="false"/>.</returns>
    public static bool DoesOwnerHaveProfession(this FarmAnimal animal, IProfession profession, bool prestiged = false)
    {
        return animal.GetOwner().HasProfession(profession, prestiged);
    }

    /// <summary>Adjusts the price of the <paramref name="animal"/> for <see cref="Profession.Breeder"/>.</summary>
    /// <param name="animal">The <see cref="FarmAnimal"/>.</param>
    /// <returns>The adjusted friendship value.</returns>
    public static double GetProducerAdjustedFriendship(this FarmAnimal animal)
    {
        return Math.Pow(Math.Sqrt(2) * animal.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
    }
}
