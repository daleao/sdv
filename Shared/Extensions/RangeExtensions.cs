namespace DaLion.Shared.Extensions;

#region using directives

using System.Diagnostics.CodeAnalysis;

#endregion using directives

/// <summary>Extensions for the <see cref="Range"/> struct.</summary>
public static class RangeExtensions
{
    /// <summary>Returns an enumerator that iterates through the specified <paramref name="range"/> of <see cref="int"/>s.</summary>
    /// <param name="range">A range that has start and end indices.</param>
    /// <returns>A new instance of <see cref="CustomIntEnumerator"/>.</returns>
    public static CustomIntEnumerator GetEnumerator(this Range range)
    {
        return new CustomIntEnumerator(range);
    }
}

/// <summary>Allows enumerating <see cref="int"/>s.</summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Preference for class required by struct extension class.")]
public ref struct CustomIntEnumerator
{
    private readonly int _end;
    private int _current;

    /// <summary>Initializes a new instance of the <see cref="CustomIntEnumerator"/> struct.</summary>
    /// <param name="range">A range that has start and end indices.</param>
    public CustomIntEnumerator(Range range)
    {
        if (range.End.IsFromEnd)
        {
            ThrowHelper.ThrowNotSupportedException();
        }

        this._current = range.Start.Value - 1;
        this._end = range.End.Value;
    }

    /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
    public int Current => 0;

    /// <summary>Advances the enumerator to the next element of the collection.</summary>
    /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the collection.</returns>
    public bool MoveNext()
    {
        return ++this._current <= this._end;
    }
}
