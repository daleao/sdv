namespace DaLion.Common.Extensions.Collections;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic enumerables of objects.</summary>
public static class EnumerableExtensions
{
    /// <summary>Apply an action to each item in <see cref="IEnumerable{T}" />.</summary>
    /// <param name="action">An action to apply.</param>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items) action(item);
    }

    /// <summary>Find the item which minimizes the given predicate.</summary>
    /// <param name="predicate">A predicate which must return <see cref="IComparable"/>.</param>
    public static T Argmin<T, U>(this IEnumerable<T> enumerable, Func<T, U> predicate) where U : IComparable
    {
        return enumerable.Aggregate((a, b) => predicate(a).CompareTo(predicate(b)) < 0 ? a : b);
    }

    /// <summary>Find the item which maximizes the given predicate.</summary>
    /// <param name="predicate">A predicate which must return <see cref="IComparable"/>.</param>
    public static T Argmax<T, U>(this IEnumerable<T> enumerable, Func<T, U> predicate) where U : IComparable
    {
        return enumerable.Aggregate((a, b) => predicate(a).CompareTo(predicate(b)) > 0 ? a : b);
    }

    /// <summary>Filter out null references.</summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : class
        => enumerable.Where(x => x is not null)!;

    /// <summary>Filter out null values.</summary>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable) where T : struct
        => enumerable.Where(e => e.HasValue).Select(e => e!.Value);
}