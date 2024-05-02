namespace DaLion.Shared.Extensions.Stardew;

/// <summary>Extensions for the <see cref="Season"/> enum.</summary>
public static class SeasonExtensions
{
    /// <summary>Gets the <see cref="Season"/> before <paramref name="season"/>.</summary>
    /// <param name="season">The <see cref="Season"/>.</param>
    /// <returns>The <see cref="Season"/> before <paramref name="season"/>.</returns>
    public static Season Previous(this Season season)
    {
        return season is Season.Spring ? Season.Winter : season - 1;
    }

    /// <summary>Gets the <see cref="Season"/> after <paramref name="season"/>.</summary>
    /// <param name="season">The <see cref="Season"/>.</param>
    /// <returns>The <see cref="Season"/> after <paramref name="season"/>.</returns>
    public static Season Next(this Season season)
    {
        return season is Season.Winter ? Season.Spring : season + 1;
    }
}
