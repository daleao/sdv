namespace DaLion.Common.Extensions.Collections;

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic collections of objects.</summary>
public static class CollectionExtensions
{
    /// <summary>Determine if a collection contains any of the objects in a sequence.</summary>
    /// <param name="candidates">The objects to search for.</param>
    public static bool ContainsAnyOf<T>(this ICollection<T> collection, params T[] candidates)
    {
        return candidates.Any(collection.Contains);
    }

    /// <summary>Determine if a collection contains any of the objects in a sequence.</summary>
    /// <param name="candidates">The objects to search for.</param>
    public static bool ContainsAnyOf<T>(this ICollection<T> collection, IEnumerable<T> candidates)
    {
        return candidates.Any(collection.Contains);
    }

    /// <summary>Determine if a collection contains any instance of the given types.</summary>
    /// <param name="types">The types to search for.</param>
    public static bool ContainsType<T>(this ICollection<T> collection, Type type)
    {
        return collection.Any(item => item is not null && item.GetType() == type);
    }

    /// <summary>Remove the first instance of a given type from a collection.</summary>
    /// <param name="type">The type to search for.</param>
    /// <param name="removed">The removed instance.</param>
    /// <returns><see langword="true"> if an instance was successfully removed, otherwise <see langword="false">.</returns>
    public static bool RemoveType<T>(this ICollection<T> collection, Type type, out T removed)
    {
        var toRemove = collection.SingleOrDefault(item => item is not null && item.GetType() == type);
        if (toRemove is not null)
        {
            removed = toRemove;
            return collection.Remove(toRemove);
        }

        removed = default;
        return false;
    }

    /// <summary>Add an item to the collection, or replace an already existing item with the new one.</summary>
    /// <param name="item">The item to add.</param>
    /// <returns><see langword="true"> if an item was added, otherwise <see langword="false">.</returns>
    public static bool AddOrReplace<T>(this ICollection<T> collection, T item)
    {
        var removed = collection.Remove(item);
        collection.Add(item);
        return !removed;
    }
}