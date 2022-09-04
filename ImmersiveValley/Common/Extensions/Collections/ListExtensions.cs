namespace DaLion.Common.Extensions.Collections;

#region using directives

using LinqFasterer;
using System;
using System.Collections.Generic;

#endregion using directives

/// <summary>Extensions for generic lists of objects.</summary>
public static class ListExtensions
{
    /// <summary>Find the index of the highest-valued item in the list.</summary>
    public static int IndexOfMax<T>(this IList<T> list) where T : IComparable
    {
        return list.IndexOf(list.MaxF()!);
    }

    /// <summary>Find the index of the lowest-valued item in the list.</summary>
    public static int IndexOfMin<T>(this IList<T> list) where T : IComparable
    {
        return list.IndexOf(list.MinF()!);
    }

    /// <summary>Move the item at position <paramref name="oldIndex"/> to position <paramref name="newIndex"/>.</summary>
    /// <param name="oldIndex">The current position.</param>
    /// <param name="newIndex">The new position.</param>
    public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
    {
        if (oldIndex == newIndex) return;

        var item = list[oldIndex];
        list.RemoveAt(oldIndex);
        if (newIndex > oldIndex) newIndex--;

        list.Insert(newIndex, item);
    }

    /// <summary>Move the specified item to position <paramref name="newIndex"/>.</summary>
    /// <param name="item">The item to be moved.</param>
    /// <param name="newIndex">The new position.</param>
    /// <returns><see langword="true"/> if a matching item was found and moved, otherwise <see langword="false"/>.</returns>
    public static bool Move<T>(this IList<T> list, T item, int newIndex)
    {
        if (item is null) return false;

        var oldIndex = list.IndexOf(item);
        if (oldIndex < 0) return false;

        list.Move(oldIndex, newIndex);
        return true;
    }

    /// <summary>Move the first item in the list to match the specified predicate to the specified new position.</summary>
    /// <param name="predicate">A delegate that identifies the item to be moved.</param>
    /// <param name="newIndex">The new position.</param>
    /// <returns><see langword="true"/> if a matching item was moved, otherwise <see langword="false"/>.</returns>
    public static bool Move<T>(this IList<T> list, Func<T, bool> predicate, int newIndex)
    {
        var toBeMoved = list.FirstOrDefaultF(predicate);
        return toBeMoved is not null && list.Move(toBeMoved, newIndex);
    }

    /// <summary>Swap the items at the two specified list indices.</summary>
    /// <param name="firstIndex">The index of the first item to be swapped.</param>
    /// <param name="secondIndex">The index of the second item to be swapped.</param>
    public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
    {
        if (firstIndex == secondIndex) return;
        (list[firstIndex], list[secondIndex]) = (list[secondIndex], list[firstIndex]);
    }

    /// <summary>Swap the positions of the two specified list items.</summary>
    /// <param name="first">The first item to be swapped.</param>
    /// <param name="second">The second item to be swapped.</param>
    public static void Swap<T>(this IList<T> list, T first, T second)
    {
        var firstIndex = list.IndexOf(first);
        var secondIndex = list.IndexOf(second);
        list.Swap(firstIndex, secondIndex);
    }
}