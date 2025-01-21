namespace DaLion.Shared.Extensions.Collections;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic collections of objects.</summary>
public static class CollectionExtensions
{
    /// <summary>Determines whether the <paramref name="collection"/> contains any of the specified <paramref name="items"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="collection"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/>.</param>
    /// <param name="items">The objects to search for.</param>
    /// <returns><see langword="true"/> if the <paramref name="collection"/> contains at least one of the specified <paramref name="items"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsAny<T>(this ICollection<T> collection, params T[] items)
    {
        return items.Any(collection.Contains);
    }

    /// <summary>
    ///     Determines whether the <paramref name="collection"/> contains any of the enumerated <paramref name="items"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <paramref name="collection"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/>.</param>
    /// <param name="items">The objects to search for.</param>
    /// <returns><see langword="true"/> if the <paramref name="collection"/> contains at least one of the specified <paramref name="items"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsAny<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        return items.Any(collection.Contains);
    }

    /// <summary>Determines whether the <paramref name="collection"/> contains all the specified <paramref name="items"/>.</summary>
    /// <typeparam name="T">The type of the elements in the <paramref name="collection"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/>.</param>
    /// <param name="items">The objects to search for.</param>
    /// <returns><see langword="true"/> if the <paramref name="collection"/> contains all <paramref name="items"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsAll<T>(this ICollection<T> collection, params T[] items)
    {
        return items.All(collection.Contains);
    }

    /// <summary>
    ///     Determines whether the <paramref name="collection"/> contains of the enumerated <paramref name="items"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <paramref name="collection"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/>.</param>
    /// <param name="items">The objects to search for.</param>
    /// <returns><see langword="true"/> if the <paramref name="collection"/> contains all <paramref name="items"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsAll<T>(this ICollection<T> collection, IEnumerable<T> items)
    {
        return items.All(collection.Contains);
    }

    /// <summary>Determines whether a <paramref name="collection"/> contains any instance of the given type.</summary>
    /// <typeparam name="T">The type of the elements in the <paramref name="collection"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/>.</param>
    /// <param name="type">The type to search for. Should be a subtype of <typeparamref name="T"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="collection"/> contains at least one element of the specified <paramref name="type"/>, otherwise <see langword="false"/>.</returns>
    public static bool ContainsType<T>(this ICollection<T> collection, Type type)
    {
        return type.IsAssignableTo(typeof(T)) && collection.Any(item => item is not null && item.GetType() == type);
    }

    /// <summary>Removes the first instance of a given type from a <paramref name="collection"/>.</summary>
    /// <typeparam name="T">The type of the elements in the <paramref name="collection"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/>.</param>
    /// <param name="type">The type to search for.</param>
    /// <param name="removed">The removed instance.</param>
    /// <returns><see langword="true"/> if an instance was successfully removed, otherwise <see langword="false"/>.</returns>
    public static bool TryRemoveType<T>(this ICollection<T> collection, Type type, out T? removed)
    {
        var toRemove = collection.FirstOrDefault(item => item?.GetType() == type);
        if (toRemove is not null)
        {
            removed = toRemove;
            return collection.Remove(toRemove);
        }

        removed = default;
        return false;
    }

    /// <summary>
    ///     Removes the first instance of each of the given <paramref name="types"/> from a
    ///     <paramref name="collection"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <paramref name="collection"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/>.</param>
    /// <param name="types">The types to search for.</param>
    public static void RemoveTypes<T>(this ICollection<T> collection, params Type[] types)
    {
        types.ForEach(t => collection.TryRemoveType(t, out _));
    }

    /// <summary>
    ///     Adds the specified <paramref name="item"/> to the <paramref name="collection"/>, or moves it to the top
    ///     if already contained.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <paramref name="collection"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/>.</param>
    /// <param name="item">The item to add.</param>
    /// <returns>
    ///     <see langword="true"/> if the <paramref name="collection"/> did not yet contain the <paramref name="item"/>,
    ///     otherwise <see langword="false"/>.
    /// </returns>
    public static bool AddOrReplace<T>(this ICollection<T> collection, T item)
    {
        var removed = collection.Remove(item);
        collection.Add(item);
        return !removed;
    }

    /// <summary>Determines whether all elements in the <paramref name="collection"/> are equal.</summary>
    /// <typeparam name="T">The type of the elements in the <paramref name="collection"/>, which should implements <see cref="IEquatable{T}"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/>.</param>
    /// <param name="comparer">Optional <see cref="IComparer{T}"/> object to define the equality between elements in the <paramref name="collection"/>.</param>
    /// <returns><see langword="true"/> if all elements in the <paramref name="collection"/> are equal, otherwise <see langword="false"/>.</returns>
    public static bool AreAllEqual<T>(this ICollection<T> collection, IEqualityComparer<T>? comparer = null)
        where T : IEquatable<T>
    {
        return collection.Distinct(comparer).Count() == 1;
    }

    /// <summary>Concatenates the elements of the <paramref name="collection"/> into a single string, using the specified <paramref name="separator"/> between each element.</summary>
    /// <typeparam name="T">The type of the elements in the <see cref="ICollection{T}"/>.</typeparam>
    /// <param name="collection">The <see cref="ICollection{T}"/> whose elements will be joined into a single string.</param>
    /// <param name="separator">The string to use as a separator between elements.</param>
    /// <returns>A single string that consists of the elements of the array, delimited by the specified separator. If the array is empty, the method returns an empty string.</returns>
    public static string Join<T>(this ICollection<T> collection, string separator)
    {
        return string.Join(separator, collection);
    }
}
