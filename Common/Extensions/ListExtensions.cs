namespace DaLion.Stardew.Common.Extensions;

#region using directives

using System.Collections.Generic;

#endregion using directives

public static class ListExtensions
{
    /// <inheritdoc cref="List{T}.AddRange"/>
    /// <param name="items">The elements to be added.</param>
    public static void AddRange<T>(this List<T> list, params T[] items)
    {
        list.AddRange(items);
    }
}