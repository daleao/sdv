namespace DaLion.Stardew.Professions.Framework.Extensions;

#region using directives

using System;
using StardewValley;

#endregion

internal static class FarmAnimalExtensions
{
    /// <summary>Affects the price of animals sold by Breeder.</summary>
    public static double GetProducerAdjustedFriendship(this FarmAnimal animal)
    {
        return Math.Pow(Math.Sqrt(2) * animal.friendshipTowardFarmer.Value / 1000, 2) + 0.5;
    }
}