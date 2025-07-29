namespace DaLion.Shared.Extensions.Collections;

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Extensions for generic enumerations of objects.</summary>
public static class EnumerableExtensions
{
    /// <summary>Applies an <paramref name="action"/> to each item in the <paramref name="enumerable"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="enumerable"/>.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
    /// <param name="action">An action to apply.</param>
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
        }
    }

    /// <summary>Finds the item which maximizes the given <paramref name="predicate"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="enumerable"/>.</typeparam>
    /// <typeparam name="TComparable">The type returned by the <paramref name="predicate"/>, which should implement <see cref="IComparable"/>.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
    /// <param name="predicate">A predicate which must return <see cref="IComparable"/>.</param>
    /// <returns>The <typeparamref name="T"/> item in the enumerable which yields the highest <typeparamref name="TComparable"/> predicate result.</returns>
    public static T? ArgMax<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> predicate)
        where TComparable : IComparable<TComparable>
    {
        var list = enumerable as IList<T> ?? enumerable.ToList();
        return !list.Any() ? default : list.Aggregate((a, b) => predicate(a).CompareTo(predicate(b)) >= 0 ? a : b);
    }

    /// <summary>Finds the item which minimizes the given <paramref name="predicate"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="enumerable"/>.</typeparam>
    /// <typeparam name="TComparable">The type returned by the <paramref name="predicate"/>, which should implement <see cref="IComparable"/>.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
    /// <param name="predicate">A predicate which must return <see cref="IComparable"/>.</param>
    /// <returns>The <typeparamref name="T"/> item in the enumerable which yields the lowest <typeparamref name="TComparable"/> predicate result.</returns>
    public static T? ArgMin<T, TComparable>(this IEnumerable<T> enumerable, Func<T, TComparable> predicate)
        where TComparable : IComparable<TComparable>
    {
        var list = enumerable as IList<T> ?? enumerable.ToList();
        return !list.Any() ? default : list.Aggregate((a, b) => predicate(a).CompareTo(predicate(b)) <= 0 ? a : b);
    }

    /// <summary>Filters out <see langword="null"/> references from the <paramref name="enumerable"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="enumerable"/>.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> that contains only the non-null references of the original.</returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        where T : class
    {
        return enumerable.Where(x => x is not null)!;
    }

    /// <summary>Filters out <see langword="null"/> values from the <paramref name="enumerable"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="enumerable"/>.</typeparam>
    /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
    /// <returns>A new <see cref="IEnumerable{T}"/> that contains only the non-null values of the original.</returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        where T : struct
    {
        return enumerable.Where(e => e.HasValue).Select(e => e!.Value);
    }

    /// <summary>Selects a random value from amongst the specified <paramref name="enumerable"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="enumerable"/>.</typeparam>
    /// <param name="enumerable">An <see cref="IEnumerable{T}"/>.</param>
    /// <param name="r">A <see cref="Random"/> number generator.</param>
    /// <returns>A random <typeparamref name="T"/> element from within <paramref name="enumerable"/>.</returns>
    public static T? Choose<T>(this IEnumerable<T> enumerable, Random? r = null)
    {
        r ??= new Random(Guid.NewGuid().GetHashCode());
        if (enumerable is ICollection<T> { Count: > 0 } collection)
        {
            return collection.ElementAt(r.Next(collection.Count));
        }

        var selected = default(T);
        var count = 0;
        foreach (var element in enumerable)
        {
            if (r.Next(++count) == 0)
            {
                selected = element;
            }
        }

        return selected;
    }

    /// <summary>Shuffles the elements in the specified <paramref name="enumerable"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="enumerable"/>.</typeparam>
    /// <param name="enumerable">An <see cref="IEnumerable{T}"/>.</param>
    /// <param name="r">A <see cref="Random"/> number generator.</param>
    /// <returns>The <paramref name="enumerable"/> with shuffled elements.</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable, Random? r = null)
    {
        r ??= new Random(Guid.NewGuid().GetHashCode());
        return enumerable.OrderBy(_ => r.Next());
    }

    /// <summary>Calculates the standard deviation of the <paramref name="values"/>.</summary>
    /// <param name="values">An <see cref="IEnumerable{T}"/> of <see cref="int"/> values.</param>
    /// <returns>The standard deviation of values in <paramref name="values"/>.</returns>
    public static double StandardDeviation(this IEnumerable<int> values)
    {
        var arr = values.ToArray();
        var avg = arr.Average();
        return Math.Sqrt(arr.Average(v => Math.Pow(v - avg, 2)));
    }

    /// <summary>Calculates the standard deviation of the <paramref name="values"/>.</summary>
    /// <param name="values">An <see cref="IEnumerable{T}"/> of <see cref="float"/> values.</param>
    /// <returns>The standard deviation of values in <paramref name="values"/>.</returns>
    public static double StandardDeviation(this IEnumerable<float> values)
    {
        var arr = values.ToArray();
        var avg = arr.Average();
        return Math.Sqrt(arr.Average(v => Math.Pow(v - avg, 2)));
    }

    /// <summary>Calculates the standard deviation of the <paramref name="values"/>.</summary>
    /// <param name="values">An <see cref="IEnumerable{T}"/> of <see cref="double"/> values.</param>
    /// <returns>The standard deviation of values in <paramref name="values"/>.</returns>
    public static double StandardDeviation(this IEnumerable<double> values)
    {
        var arr = values.ToArray();
        var avg = arr.Average();
        return Math.Sqrt(arr.Average(v => Math.Pow(v - avg, 2)));
    }

    /// <summary>Calculates the standard deviation the <paramref name="values"/>.</summary>
    /// <typeparam name="TEnum">A type of <see cref="Enum"/>.</typeparam>
    /// <param name="values">An <see cref="IEnumerable{T}"/> of <see cref="int"/> values.</param>
    /// <returns>The standard deviation of values in <paramref name="values"/>.</returns>
    public static double StandardDeviation<TEnum>(this IEnumerable<TEnum> values)
        where TEnum : Enum
    {
        var arr = values.Select(v => (int)(object)v).ToArray();
        var avg = arr.Average();
        return Math.Sqrt(arr.Average(v => Math.Pow(v - avg, 2)));
    }
}
