namespace DaLion.Common.Extensions;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic objects.</summary>
public static class GenericExtensions
{
    /// <summary>Determine if the instance is contained by the collection.</summary>
    /// <param name="collection">A sequence of <typeparamref name="T" /> objects.</param>
    public static bool IsIn<T>(this T item, params T[] collection)
    {
        return collection.Contains(item);
    }

    /// <summary>Determine if the instance is contained by the collection.</summary>
    /// <param name="collection">A sequence of <typeparamref name="T" /> objects.</param>
    public static bool IsIn<T>(this T item, IEnumerable<T> collection)
    {
        return collection.Contains(item);
    }

    /// <summary>Enumerate this and specified items.</summary>
    /// <param name="otherItems">Other items to add to the collection.</param>
    public static IEnumerable<T> Collect<T>(this T item, params T[] otherItems)
    {
        yield return item;
        foreach (var otherItem in otherItems) yield return otherItem;
    }
}