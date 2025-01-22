namespace DaLion.Shared.Extensions.Collections;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic <see cref="List{T}"/> of objects.</summary>
public static class ListExtensions
{
    /// <summary>Removes <see langword="null"/> references from the <paramref name="list"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">A <see cref="List{T}"/> of <typeparamref name="T"/>s.</param>
    public static void RemoveWhereNull<T>(this List<T?> list)
        where T : class
    {
        list.RemoveAll(item => item is null);
    }

    /// <summary>Sorts the <paramref name="list"/> in reverse order.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>. <typeparamref name="T"/> must be <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="list">A <see cref="List{T}"/> of <see cref="IComparable{T}"/>s.</param>
    public static void SortDescending<T>(this List<T> list)
        where T : IComparable<T>
    {
        list.Sort();
        list.Reverse();
    }

    /// <summary>Determines whether the specified <paramref name="index"/> is within the bounds of the <paramref name="list"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="index">The <see cref="int"/> index.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is greater than or equal to zero, and less than the size of the <paramref name="list"/>.</returns>
    public static bool IsIndexInBounds<T>(this IList<T> list, int index)
    {
        return index >= 0 && index < list.Count;
    }

    /// <summary>Finds the index of the highest-valued item in the <paramref name="list"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>, which should implement <see cref="IComparable"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <returns>The integer index of the element in the <paramref name="list"/> wit the highest value.</returns>
    public static int IndexOfMax<T>(this IList<T> list)
        where T : IComparable<T>, new()
    {
        return list.IndexOf(list.Max() ?? new T());
    }

    /// <summary>Finds the index of the lowest-valued item in the <paramref name="list"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>, which should implement <see cref="IComparable"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <returns>The integer index of the element in the <paramref name="list"/> wit the lowest value.</returns>
    public static int IndexOfMin<T>(this IList<T> list)
        where T : IComparable<T>, new()
    {
        return list.IndexOf(list.Min() ?? new T());
    }

    /// <summary>Moves the item at position <paramref name="oldIndex"/> to position <paramref name="newIndex"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="oldIndex">The current position.</param>
    /// <param name="newIndex">The new position.</param>
    public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
    {
        if (oldIndex == newIndex)
        {
            return;
        }

        var item = list[oldIndex];
        list.RemoveAt(oldIndex);
        if (newIndex > oldIndex)
        {
            newIndex--;
        }

        list.Insert(newIndex, item);
    }

    /// <summary>Moves the specified <paramref name="item"/> to position <paramref name="newIndex"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="item">The item to be moved.</param>
    /// <param name="newIndex">The new position.</param>
    /// <returns><see langword="true"/> if a matching item was found and moved, otherwise <see langword="false"/>.</returns>
    public static bool Move<T>(this IList<T> list, T item, int newIndex)
    {
        if (item is null)
        {
            return false;
        }

        var oldIndex = list.IndexOf(item);
        if (oldIndex < 0)
        {
            return false;
        }

        list.Move(oldIndex, newIndex);
        return true;
    }

    /// <summary>
    ///     Moves the first item in the list that matches the <paramref name="predicate"/> to position
    ///     <paramref name="newIndex"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="predicate">A delegate that identifies the item to be moved.</param>
    /// <param name="newIndex">The new position.</param>
    /// <returns><see langword="true"/> if a matching item was moved, otherwise <see langword="false"/>.</returns>
    public static bool Move<T>(this IList<T> list, Func<T, bool> predicate, int newIndex)
    {
        var toBeMoved = list.FirstOrDefault(predicate);
        return toBeMoved is not null && list.Move(toBeMoved, newIndex);
    }

    /// <summary>Swaps the items at the specified positions.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="firstIndex">The index of the first item to be swapped.</param>
    /// <param name="secondIndex">The index of the second item to be swapped.</param>
    public static void Swap<T>(this IList<T> list, int firstIndex, int secondIndex)
    {
        if (firstIndex == secondIndex)
        {
            return;
        }

        (list[firstIndex], list[secondIndex]) = (list[secondIndex], list[firstIndex]);
    }

    /// <summary>Swap the positions of the specified items.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="first">The first item to be swapped.</param>
    /// <param name="second">The second item to be swapped.</param>
    /// <exception cref="InvalidOperationException">If either <paramref name="first"/> or <paramref name="second"/> are not found in the <paramref name="list"/>.</exception>
    public static void Swap<T>(this IList<T> list, T first, T second)
    {
        var firstIndex = list.IndexOf(first);
        if (firstIndex < 0)
        {
            ThrowHelper.ThrowInvalidOperationException($"The item {first} was not found.");
        }

        var secondIndex = list.IndexOf(second);
        if (firstIndex < 0)
        {
            ThrowHelper.ThrowInvalidOperationException($"The item {second} was not found.");
        }

        list.Swap(firstIndex, secondIndex);
    }

    /// <summary>Shifts all items <paramref name="count"/> units to the right.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="count">The number of shifts to perform.</param>
    public static void ShiftRight<T>(this IList<T> list, int count)
    {
        count %= list.Count;
        if (count == 0)
        {
            return;
        }

        var array = list.ToArray();
        for (var i = 0; i < count; i++)
        {
            array.ShiftRight();
        }

        for (var i = 0; i < list.Count; i++)
        {
            list[i] = array[i];
        }
    }

    /// <summary>Shifts all items <paramref name="count"/> units to the left.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="count">The number of shifts to perform.</param>
    public static void ShiftLeft<T>(this IList<T> list, int count)
    {
        count %= list.Count;
        if (count == 0)
        {
            return;
        }

        var array = list.ToArray();
        for (var i = 0; i < count; i++)
        {
            array.ShiftLeft();
        }

        for (var i = 0; i < list.Count; i++)
        {
            list[i] = array[i];
        }
    }

    /// <summary>
    ///     Shifts the elements in <paramref name="list"/> to the left until they match the specified
    ///     <paramref name="pattern"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="pattern">A pattern of elements to match.</param>
    /// <returns>The number of left shifts that were performed.</returns>
    /// <exception cref="InvalidOperationException">If the specified <paramref name="pattern"/> does not contain the exact same elements as the <paramref name="list"/>.</exception>
    public static int ShiftUntilEqual<T>(this IList<T> list, IList<T> pattern)
    {
        if (!list.IsPermutationOf(pattern))
        {
            ThrowHelper.ThrowInvalidOperationException("The specified pattern is not a permutation of the list.");
        }

        var shifts = 0;
        while (!list.SequenceEqual(pattern))
        {
            list.ShiftLeft(1);
            shifts++;
        }

        return shifts;
    }

    /// <summary>
    ///     Shifts the elements in <paramref name="list"/> to the left until the specified <paramref name="element"/> is
    ///     at index zero.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="element">The element to be placed at index zero.</param>
    /// <returns>The number of left shifts that were performed.</returns>
    /// <exception cref="InvalidOperationException">If the specified <paramref name="element"/> is not found in the <paramref name="list"/>.</exception>
    public static int ShiftUntilStartsWith<T>(this IList<T> list, T element)
    {
        var shifts = list.IndexOf(element);
        switch (shifts)
        {
            case < 0:
                ThrowHelper.ThrowInvalidOperationException("The specified element is not in the list.");
                break;
            case 0:
                return 0;
            default:
                list.ShiftLeft(shifts);
                break;
        }

        return shifts;
    }

    /// <summary>Determines whether the <paramref name="list"/> is a permutation of some <paramref name="other"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="list"/>.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="other">Some other list.</param>
    /// <returns><see langword="true"/> if both <paramref name="list"/> and <paramref name="other"/> contain the exact same elements in any order, otherwise <see langword="false"/>.</returns>
    public static bool IsPermutationOf<T>(this IList<T> list, IList<T> other)
    {
        if (list.Count != other.Count)
        {
            return false;
        }

        var l1 = list.ToLookup(t => t);
        var l2 = other.ToLookup(t => t);
        return l1.Count == l2.Count &&
               l1.All(group => l2.Contains(group.Key) && l2[group.Key].Count() == group.Count());
    }

    /// <summary>Chooses a random element from the <paramref name="list"/>.</summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="r">A <see cref="Random"/> number generator.</param>
    /// <returns>A random element from the <paramref name="list"/>.</returns>
    public static T Choose<T>(this IList<T> list, Random? r = null)
    {
        r ??= new Random(Guid.NewGuid().GetHashCode());
        return list[r.Next(list.Count)];
    }

    /// <summary>Shuffles the elements in the <paramref name="list"/>.</summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The <see cref="IList{T}"/>.</param>
    /// <param name="r">A <see cref="Random"/> number generator.</param>
    public static void Shuffle<T>(this IList<T> list, Random? r = null)
    {
        r ??= new Random(Guid.NewGuid().GetHashCode());
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = r.Next(i + 1);
            (list[j], list[i]) = (list[i], list[j]);
        }
    }
}
