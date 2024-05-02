namespace DaLion.Shared.Extensions.Memory;

#region using directives

using System.Diagnostics.CodeAnalysis;

#endregion using directives

/// <summary>Extensions for <see langword="ref struct"/> instances like <see cref="Memory{T}"/> and <see cref="Span{T}"/>.</summary>
public static class MemoryExtensions
{
    /// <summary>Splits a <see cref="ReadOnlySpan{T}"/> of <see cref="char"/> into its constituent slices based on the specified <paramref name="splitter"/> <see cref="char"/>.</summary>
    /// <param name="span">A <see cref="ReadOnlySpan{T}"/> of <see cref="char"/>s.</param>
    /// <param name="splitter">A <see cref="char"/>s that will be used to split the <paramref name="span"/>.</param>
    /// <returns>A <see cref="SpanSplitter"/> object that can be used to iterate through the substrings within the <see cref="ReadOnlySpan{T}"/>.</returns>
    public static SpanSplitter Split(this ReadOnlySpan<char> span, char splitter = ' ') => new(span, splitter);
}

/// <summary>Used for enumerating and accessing slices of <see cref="ReadOnlySpan{T}"/>.</summary>
[SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "Preference for struct required by extension class.")]
public ref struct SpanSplitter
{
    private readonly char _splitter;
    private ReadOnlySpan<char> _span;

    /// <summary>Initializes a new instance of the <see cref="SpanSplitter"/> struct.</summary>
    /// <param name="span">A <see cref="ReadOnlySpan{T}"/> of <see cref="char"/>s.</param>
    /// <param name="splitter">A <see cref="char"/>s that will be used to split the <paramref name="span"/>.</param>
    public SpanSplitter(ReadOnlySpan<char> span, char splitter)
    {
        this._span = span;
        this._splitter = splitter;
        this.Current = default;
        if (span.Length == 0)
        {
            return;
        }

        this.Length++;
        var index = span.IndexOf(splitter);
        if (index < 0)
        {
            return;
        }

        while (span.Length > 0 && index >= 0)
        {
            this.Length++;
            span = span[(index + 1)..];
            index = span.IndexOf(splitter);
        }
    }

    /// <summary>Gets the slice in the <see cref="ReadOnlySpan{T}"/> at the current position of the enumerator.</summary>
    public ReadOnlySpan<char> Current { get; private set; }

    /// <summary>Gets the number of Slices in the <see cref="ReadOnlySpan{T}"/>.</summary>
    public int Length { get; } = 0;

    /// <summary>Gets the slice at the specified <paramref name="index"/>.</summary>
    /// <param name="index">A <see cref="int"/> index.</param>
    public ReadOnlySpan<char> this[int index]
    {
        get
        {
            if (index >= this._span.Length)
            {
                return ThrowHelper.ThrowArgumentOutOfRangeException<string>();
            }

            if (index == 0)
            {
                return this._span[..this._span.IndexOf(this._splitter)];
            }

            if (index == this.Length - 1)
            {
                return this._span[(this._span.LastIndexOf(this._splitter) + 1)..];
            }

            var mid = this.Length / 2;
            var span = this._span;
            if (index <= mid)
            {
                span = span[(span.IndexOf(this._splitter) + 1)..];
                for (var i = 1; i <= index; i++)
                {
                    if (i == index)
                    {
                        return span[..span.IndexOf(this._splitter)];
                    }

                    span = span[(span.IndexOf(this._splitter) + 1)..];
                }
            }
            else
            {
                span = span[..span.LastIndexOf(this._splitter)];
                for (var i = this.Length - 2; i >= index; i--)
                {
                    if (i == index)
                    {
                        return span[(span.LastIndexOf(this._splitter) + 1)..];
                    }

                    span = span[..span.LastIndexOf(this._splitter)];
                }
            }

            return string.Empty;
        }
    }

    /// <summary>Returns an enumerator that iterates through the <see cref="ReadOnlySpan{T}"/> of <see cref="char"/>s.</summary>
    /// <returns>A <see cref="SpanSplitter"/> object that can be used to iterate through the substrings within the <see cref="Span{T}"/>.</returns>
    public SpanSplitter GetEnumerator() => this;

    /// <summary>Advances the enumerator to the next element of the collection.</summary>
    /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the collection.</returns>
    public bool MoveNext()
    {
        if (this._span.Length == 0)
        {
            return false;
        }

        var index = this._span.IndexOf(this._splitter);
        if (index < 0)
        {
            this.Current = this._span;
            this._span = ReadOnlySpan<char>.Empty;
        }
        else
        {
            this.Current = this._span[..index];
            this._span = this._span[(index + 1)..];
        }

        return true;
    }

    /// <summary>Advances the enumerator by <paramref name="count"/> elements.</summary>
    /// <param name="count">The number of advancements to perform.</param>
    public void MoveNext(int count)
    {
        while (count-- > 0 && this.MoveNext())
        {
        }
    }
}
