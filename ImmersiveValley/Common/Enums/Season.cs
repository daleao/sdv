namespace DaLion.Common.Enums;

#region using directives

using DaLion.Common.Exceptions;
using NetEscapades.EnumGenerators;

#endregion using directives

/// <summary>An in-game Season.</summary>
[EnumExtensions]
public enum Season
{
    /// <summary>The Spring season.</summary>
    Spring,

    /// <summary>The Summer season.</summary>
    Summer,

    /// <summary>The Fall season.</summary>
    Fall,

    /// <summary>The Winter season.</summary>
    Winter,
}

/// <summary>Extensions for the <see cref="Season"/> enum.</summary>
public static partial class SeasonExtensions
{
    /// <summary>Gets the <see cref="Season"/> before <paramref name="season"/>.</summary>
    /// <param name="season">The <see cref="Season"/>.</param>
    /// <returns>The <see cref="Season"/> before <paramref name="season"/>.</returns>
    public static Season Previous(this Season season)
    {
        return season - 1;
    }

    /// <summary>Gets the <see cref="Season"/> after <paramref name="season"/>.</summary>
    /// <param name="season">The <see cref="Season"/>.</param>
    /// <returns>The <see cref="Season"/> after <paramref name="season"/>.</returns>
    public static Season Next(this Season season)
    {
        return season + 1;
    }

    /// <summary>Gets the current in-game <see cref="Season"/>.</summary>
    /// <returns>The current <see cref="Season"/>n.</returns>
    public static Season Current()
    {
        if (!TryParseIgnoreCase(Game1.currentSeason, out var current))
        {
            ThrowHelperExtensions.ThrowUnexpectedEnumValueException(Game1.currentSeason);
        }

        return current;
    }

    /// <summary>Gets the previous in-game <see cref="Season"/>.</summary>
    /// <returns>The previous <see cref="Season"/>n.</returns>
    public static Season Previous()
    {
        if (!TryParseIgnoreCase(Game1.currentSeason, out var current))
        {
            ThrowHelperExtensions.ThrowUnexpectedEnumValueException(Game1.currentSeason);
        }

        return current - 1;
    }

    /// <summary>Gets the next in-game <see cref="Season"/>.</summary>
    /// <returns>The next <see cref="Season"/>n.</returns>
    public static Season Next()
    {
        if (!TryParseIgnoreCase(Game1.currentSeason, out var current))
        {
            ThrowHelperExtensions.ThrowUnexpectedEnumValueException(Game1.currentSeason);
        }

        return current + 1;
    }
}
