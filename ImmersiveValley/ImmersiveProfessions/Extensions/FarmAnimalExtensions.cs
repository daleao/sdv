namespace DaLion.Stardew.Professions.Extensions;

#region using directives

using System;
using StardewValley;

#endregion

/// <summary>Extensions for the <see cref="FarmAnimal"/> class.</summary>
public static class FarmAnimalExtensions
{
    /// <summary>Affects the price of animals sold by Breeder.</summary>
    public static double GetProducerAdjustedFriendship(this FarmAnimal animal)
    {
        return Math.Pow(Math.Sqrt(2) * animal.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
    }
}