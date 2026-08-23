namespace DaLion.Professions.Framework.Extensions;

#region using directives

using DaLion.Shared.Extensions;
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
    /// <returns>The adjusted sale value.</returns>
    internal static float GetBreederAdjustedPrice(this FarmAnimal animal)
    {
        if (!animal.isAdult())
        {
            return 1f;
        }

        var inheritedPotential = Data.ReadAs<int>(animal, DataKeys.InheritedPotential);

        var potentialMultiplier = inheritedPotential < 5000
            ? (float)Math.Pow(10d, Math.Pow(inheritedPotential / 5000d, 0.75d))
            : (float)(10d + (7.5 * Math.Log(10d) / 5000d * (inheritedPotential - 5000d)));

        const int halfLifeDays = 168;
        var daysSinceAdult = Math.Max(animal.age.Value - animal.GetAnimalData().DaysToMature, 0);
        var ageMultiplier = (float)Math.Pow(2d, -daysSinceAdult / halfLifeDays);

        var breedMultiplier = 1f;
        if (animal.type.Value.ContainsAnyOf(Lookups.AnimalReproductiveTypes.Mammals))
        {
            var pregnancies = Data.ReadAs<int>(animal, DataKeys.Pregnancies);
            breedMultiplier = (float)Math.Max(1d / (1d + (pregnancies * 0.1)), 0.5);
        }
        else if (animal.type.Value.ContainsAnyOf(Lookups.AnimalReproductiveTypes.EggLayers))
        {
            var eggsLaid = Data.ReadAs<int>(animal, DataKeys.EggsLaid);
            breedMultiplier = (float)Math.Max(1d - (5e-4d * eggsLaid), 0.75d);
        }

        return potentialMultiplier * ageMultiplier * breedMultiplier;
    }
}
