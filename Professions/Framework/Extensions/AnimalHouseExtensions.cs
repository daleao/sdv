namespace DaLion.Professions.Framework.Extensions;

#region using directives

using System.Linq;

#endregion using directives

/// <summary>Extensions for the <see cref="AnimalHouse"/> class.</summary>
internal static class AnimalHouseExtensions
{
    /// <summary>Determines the number of animals in <paramref name="house"/> that can have pregnancy.</summary>
    /// <param name="house">The <see cref="AnimalHouse"/>.</param>
    /// <returns>The number of animals in <paramref name="house"/> that can have pregnancy.</returns>
    internal static int AnimalsThatCanHavePregnancy(this AnimalHouse house)
    {
        return house.Animals.Values.Where(a => a.CanHavePregnancy()).Count();
    }
}
