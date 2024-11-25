namespace DaLion.Shared.Extensions;

#region using directives

using System.Linq;
using DaLion.Shared.Exceptions;

#endregion using directives

/// <summary>Extensions for generic arrays of objects.</summary>
public static class ArrayExtensions
{
    /// <summary>Performs element-wise addition.</summary>
    /// <param name="a">The first <see cref="Array"/>.</param>
    /// <param name="b">The second <see cref="Array"/>.</param>
    /// <returns>A new array whose elements are the sum of the corresponding elements from <paramref name="a"/> and <paramref name="b"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="a"/> and <paramref name="b"/> are not equal in length.</exception>
    public static int[] ElementwiseAdd(this int[] a, int[] b)
    {
        if (a.Length != b.Length)
        {
            ThrowHelper.ThrowInvalidOperationException("Arrays must have the same length for element-wise addition.");
        }

        return a.Zip(b, (x, y) => x + y).ToArray();
    }

    /// <summary>Performs element-wise addition.</summary>
    /// <param name="a">The first <see cref="Array"/>.</param>
    /// <param name="b">The second <see cref="Array"/>.</param>
    /// <returns>A new array whose elements are the sum of the corresponding elements from <paramref name="a"/> and <paramref name="b"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="a"/> and <paramref name="b"/> are not equal in length.</exception>
    public static float[] ElementwiseAdd(this float[] a, float[] b)
    {
        if (a.Length != b.Length)
        {
            ThrowHelper.ThrowInvalidOperationException("Arrays must have the same length for element-wise addition.");
        }

        return a.Zip(b, (x, y) => x + y).ToArray();
    }

    /// <summary>Performs element-wise addition.</summary>
    /// <param name="a">The first <see cref="Array"/>.</param>
    /// <param name="b">The second <see cref="Array"/>.</param>
    /// <returns>A new array whose elements are the sum of the corresponding elements from <paramref name="a"/> and <paramref name="b"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="a"/> and <paramref name="b"/> are not equal in length.</exception>
    public static double[] ElementwiseAdd(this double[] a, double[] b)
    {
        if (a.Length != b.Length)
        {
            ThrowHelper.ThrowInvalidOperationException("Arrays must have the same length for element-wise addition.");
        }

        return a.Zip(b, (x, y) => x + y).ToArray();
    }

    /// <summary>Performs element-wise multiplication.</summary>
    /// <param name="a">The first <see cref="Array"/>.</param>
    /// <param name="b">The second <see cref="Array"/>.</param>
    /// <returns>A new array whose elements are the product of the corresponding elements from <paramref name="a"/> and <paramref name="b"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="a"/> and <paramref name="b"/> are not equal in length.</exception>
    public static int[] ElementwiseMultiply(this int[] a, int[] b)
    {
        if (a.Length != b.Length)
        {
            ThrowHelper.ThrowInvalidOperationException("Arrays must have the same length for element-wise multiplication.");
        }

        return a.Zip(b, (x, y) => x * y).ToArray();
    }

    /// <summary>Performs element-wise multiplication.</summary>
    /// <param name="a">The first <see cref="Array"/>.</param>
    /// <param name="b">The second <see cref="Array"/>.</param>
    /// <returns>A new array whose elements are the product of the corresponding elements from <paramref name="a"/> and <paramref name="b"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="a"/> and <paramref name="b"/> are not equal in length.</exception>
    public static float[] ElementwiseMultiply(this float[] a, float[] b)
    {
        if (a.Length != b.Length)
        {
            ThrowHelper.ThrowInvalidOperationException("Arrays must have the same length for element-wise multiplication.");
        }

        return a.Zip(b, (x, y) => x * y).ToArray();
    }

    /// <summary>Performs element-wise multiplication.</summary>
    /// <param name="a">The first <see cref="Array"/>.</param>
    /// <param name="b">The second <see cref="Array"/>.</param>
    /// <returns>A new array whose elements are the product of the corresponding elements from <paramref name="a"/> and <paramref name="b"/>.</returns>
    /// <exception cref="InvalidOperationException">If <paramref name="a"/> and <paramref name="b"/> are not equal in length.</exception>
    public static double[] ElementwiseMultiply(this double[] a, double[] b)
    {
        if (a.Length != b.Length)
        {
            ThrowHelper.ThrowInvalidOperationException("Arrays must have the same length for element-wise multiplication.");
        }

        return a.Zip(b, (x, y) => x * y).ToArray();
    }

    /// <summary>Sorts the <paramref name="array"/> in reverse order.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="array"/>. <paramref name="T"/> must be <see cref="IComparable{T}"/>.</typeparam>
    /// <param name="array">An array of <see cref="IComparable{T}"/>s.</param>
    public static void SortDescending<T>(this T[] array)
        where T : IComparable<T>
    {
        Array.Sort(array);
        Array.Reverse(array);
    }

    /// <summary>Determines whether the specified <paramref name="index"/> is within the bounds of the <paramref name="array"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="array"/>.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="index">The <see cref="int"/> index.</param>
    /// <returns><see langword="true"/> if <paramref name="index"/> is greater than or equal to zero, and less than the size of the <paramref name="array"/>.</returns>
    public static bool IsIndexInBounds<T>(this T[] array, int index)
    {
        return index >= 0 && index < array.Length;
    }

    /// <summary>Gets a sub-array of <paramref name="length"/> starting at <paramref name="offset"/>.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="offset">The starting index.</param>
    /// <param name="length">The length of the sub-array.</param>
    /// <returns>A new array formed by taking <paramref name="length"/> elements of the original after skipping <paramref name="offset"/>.</returns>
    public static T[] SubArray<T>(this T[] array, int offset, int length)
    {
        if (length - offset > array.Length)
        {
            ThrowHelperExtensions.ThrowIndexOutOfRangeException();
        }

        return array.Skip(offset).Take(length).ToArray();
    }

    /// <summary>Shifts all elements of the <paramref name="array"/> one unit to the right.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <remarks>The last element of the original array becomes the first element of the shifted array.</remarks>
    public static void ShiftRight<T>(this T[] array)
    {
        var temp = array[^1];
        Array.Copy(array, 0, array, 1, array.Length - 1);
        array[0] = temp;
    }

    /// <summary>Shifts all elements of the <paramref name="array"/> <paramref name="count"/> units to the right.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="count">The number of shifts to perform.</param>
    public static void ShiftRight<T>(this T[] array, int count)
    {
        count %= array.Length;
        if (count == 0)
        {
            return;
        }

        for (var i = 0; i < count; i++)
        {
            array.ShiftRight();
        }
    }

    /// <summary>Shifts all elements of the <paramref name="array"/> one unit to the left.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    public static void ShiftLeft<T>(this T[] array)
    {
        var temp = array[0];
        Array.Copy(array, 1, array, 0, array.Length - 1);
        array[^1] = temp;
    }

    /// <summary>Shifts all elements of the <paramref name="array"/> <paramref name="count"/> units to the left.</summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="count">The number of shifts to perform.</param>
    public static void ShiftLeft<T>(this T[] array, int count)
    {
        count %= array.Length;
        if (count == 0)
        {
            return;
        }

        for (var i = 0; i < count; i++)
        {
            array.ShiftLeft();
        }
    }

    /// <summary>Chooses a random element from the <paramref name="array"/>.</summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="array">The array.</param>
    /// <param name="r">A <see cref="Random"/> number generator.</param>
    /// <returns>A random element from the <paramref name="array"/>.</returns>
    /// <exception cref="IndexOutOfRangeException">If <paramref name="array"/> is empty.</exception>
    public static T Choose<T>(this T[] array, Random? r = null)
    {
        r ??= new Random(Guid.NewGuid().GetHashCode());
        return array[r.Next(array.Length)];
    }
}
