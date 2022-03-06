namespace DaLion.Stardew.Common.Extensions;

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

    /// <summary>Find the item in the collection which minimizes the given predicate.</summary>
    /// <param name="predicate">A predicate which must return <see cref="IComparable"/>.</param>
    public static T Argmin<T, U>(this IEnumerable<T> collection, Func<T, U> predicate) where U : IComparable
    {
        return collection.Aggregate((a, b) => predicate(a).CompareTo(predicate(b)) < 0 ? a : b);
    }

    /// <summary>Find the item in the collection which maximizes the given predicate.</summary>
    /// <param name="predicate">A predicate which must return <see cref="IComparable"/>.</param>
    public static T Argmax<T, U>(this IEnumerable<T> collection, Func<T, U> predicate) where U : IComparable
    {
        return collection.Aggregate((a, b) => predicate(a).CompareTo(predicate(b)) > 0 ? a : b);
    }
}