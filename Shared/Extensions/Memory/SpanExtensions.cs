namespace DaLion.Shared.Extensions.Memory;

#region using directives

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#endregion using directives

/// <summary>Extensions for the <see cref="ReadOnlySpan{T}"/> struct.</summary>
public static class SpanExtensions
{
    /// <summary>Gets the index of the first whitespace character.</summary>
    /// <param name="span">A <see cref="ReadOnlySpan{T}"/> of <see cref="char"/>.</param>
    /// <returns>The index of the first whitespace character, or -1 if not found.</returns>
    public static int IndexOfWhitespace(this ReadOnlySpan<char> span)
    {
        for (var i = 0; i < span.Length; i++)
        {
            if (char.IsWhiteSpace(span[i]))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>Reverses the sequence of the elements in the entire span.</summary>
    /// <typeparam name="T">The type contained in the span.</typeparam>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/>.</param>
    public static void Reverse<T>(this ReadOnlySpan<T?> span)
    {
        if (span.Length <= 1)
        {
            return;
        }

        ref var first = ref MemoryMarshal.GetReference(span);
        ref var last = ref Unsafe.Add(ref Unsafe.Add(ref first, span.Length), -1);
        do
        {
            (first, last) = (last, first);
            first = ref Unsafe.Add(ref first, 1);
            last = ref Unsafe.Add(ref last, -1);
        }
        while (Unsafe.IsAddressLessThan(ref first, ref last));
    }
}
