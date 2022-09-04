using System;

namespace DaLion.Common.Extensions;

#region using directives

using LinqFasterer;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic arrays of objects.</summary>
public static class ArrayExtensions
{
    /// <summary>Determine if all objects in the array are equal.</summary>
    public static bool AreAllEqual<T>(this T[] array) where T : IEquatable<T>
    {
        var first = array[0];
        return array.SkipF(1).All(i => first.Compare(i));
    }

    /// <summary>Get a sub-array from the instance.</summary>
    /// <param name="offset">The starting index.</param>
    /// <param name="length">The length of the sub-array.</param>
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        return array.SkipF(offset).TakeF(length).ToArrayF();
    }
}