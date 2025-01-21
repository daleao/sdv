namespace DaLion.Shared.Extensions.Collections;

#region using directives

using System.Collections.Generic;

#endregion using directives

/// <summary>Extensions for generic <see cref="Stack{T}"/> of objects.</summary>
public static class StackExtensions
{
    /// <summary>Pops multiple elements off the top of the <paramref name="stack"/>.</summary>
    /// <typeparam name="T">The type of elements in the <paramref name="stack"/>.</typeparam>
    /// <param name="stack">The <see cref="Stack{T}"/>.</param>
    /// <param name="n">The number of elements to pop.</param>
    /// <returns>A <see cref="List{T}"/> with the top <paramref name="n"/> elements in the <paramref name="stack"/>.</returns>
    public static List<T> PopRange<T>(this Stack<T> stack, int n)
    {
        var result = new List<T>(n);
        while (n-- > 0 && stack.Count > 0)
        {
            result.Add(stack.Pop());
        }

        return result;
    }
}
