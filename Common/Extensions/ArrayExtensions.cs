namespace DaLion.Stardew.Common.Extensions;

#region using directives

using System.Linq;

#endregion using directives

/// <summary>Extensions for generic arrays of objects.</summary>
public static class ArrayExtensions
{
    /// <summary>Get a sub-array from the instance.</summary>
    /// <param name="offset">The starting index.</param>
    /// <param name="length">The length of the sub-array.</param>
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        return array.Skip(offset).Take(length).ToArray();
    }
}