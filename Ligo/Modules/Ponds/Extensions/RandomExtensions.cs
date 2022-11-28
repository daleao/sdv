namespace DaLion.Ligo.Modules.Ponds.Extensions;

/// <summary>Extensions for the <see cref="Random"/> class.</summary>
internal static class RandomExtensions
{
    /// <summary>Gets the item index of a random algae.</summary>
    /// <param name="random">The <see cref="Random"/> number generator.</param>
    /// <param name="bias">A particular type of algae that should be favored.</param>
    /// <returns>The <see cref="int"/> index of an algae <see cref="Item"/>.</returns>
    internal static int NextAlgae(this Random random, int? bias = null)
    {
        if (bias.HasValue && random.NextDouble() > 2d / 3d)
        {
            return bias.Value;
        }

        return random.NextDouble() switch
        {
            > 2d / 3d => Constants.GreenAlgaeIndex,
            > 1d / 3d => Constants.SeaweedIndex,
            _ => Constants.WhiteAlgaeIndex,
        };
    }
}
