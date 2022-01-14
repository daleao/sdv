namespace DaLion.Stardew.Common.Extensions;

#region using directives

using System;
using System.Collections.Generic;

#endregion using directives

public static class EnumerableExtensions
{
    /// <summary>Apply an action to each item in <see cref="IEnumerable{T}" />.</summary>
    /// <param name="action">An action to apply.</param>
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items) action(item);
    }
}