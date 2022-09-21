namespace DaLion.Common.Extensions;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic objects.</summary>
public static class GenericExtensions
{
    /// <summary>Determines whether the <paramref name="value"/> is equal to an<paramref name="other"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="other">Some other value with which to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> and <paramref name="other"/> are equal with respect to the default <see cref="EqualityComparer{T}"/>, otherwise <see langword="false"/>.</returns>
    public static bool Compare<T>(this T value, T other)
        where T : IEquatable<T>
    {
        return EqualityComparer<T>.Default.Equals(value, other);
    }

    /// <summary>Determines whether the <paramref name="value"/> is equal to any of the <paramref name="candidates"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="candidates">Some candidates to check.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is equal to at least one of the <paramref name="candidates"/> with respect to the default <see cref="EqualityComparer{T}"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsAnyOf<T>(this T value, params T[] candidates)
    {
        return candidates.Contains(value);
    }

    /// <summary>Determines whether <paramref name="value"/> is equal to any of the enumerated <paramref name="candidates"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="candidates">The candidates to check.</param>
    /// <returns><see langword="true"/> if <paramref name="value"/> is equal to at least one of the <paramref name="candidates"/> with respect to the default <see cref="EqualityComparer{T}"/>, otherwise <see langword="false"/>.</returns>
    public static bool IsAnyOf<T>(this T value, IEnumerable<T> candidates)
    {
        return candidates.Contains(value);
    }

    /// <summary>Enumerates the <paramref name="value"/> and <paramref name="others"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="others">Some other objects to collect.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> combining <paramref name="value"/> and <paramref name="others"/>.</returns>
    public static IEnumerable<T> Collect<T>(this T value, params T[] others)
    {
        yield return value;
        foreach (var next in others)
        {
            yield return next;
        }
    }

    /// <summary>Enumerates the <paramref name="value"/> along with the enumerated <paramref name="others"/>.</summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="others">Some other enumeration to collect.</param>
    /// <returns>A <see cref="IEnumerable{T}"/> combining <paramref name="value"/> and <paramref name="others"/>.</returns>
    public static IEnumerable<T> Collect<T>(this T value, IEnumerable<T> others)
    {
        yield return value;
        foreach (var next in others)
        {
            yield return next;
        }
    }
}
