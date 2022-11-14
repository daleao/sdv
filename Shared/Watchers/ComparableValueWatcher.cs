namespace DaLion.Shared.Watchers;

#region using directives

using System;
using System.Collections.Generic;

#endregion using directives

/// <summary>A watcher which detects changes to a value using a specified <see cref="IEqualityComparer{T}"/> instance.</summary>
/// <typeparam name="TValue">A comparable value type.</typeparam>
internal class ComparableValueWatcher<TValue> : IValueWatcher<TValue>
{
    /// <summary>Get the current value.</summary>
    private readonly Func<TValue> _getValue;

    /// <summary>The equality comparer.</summary>
    private readonly IEqualityComparer<TValue> _comparer;

    /// <summary>Initializes a new instance of the <see cref="ComparableValueWatcher{TValue}"/> class.</summary>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="getValue">Get the current value.</param>
    /// <param name="comparer">The equality comparer which indicates whether two values are the same.</param>
    public ComparableValueWatcher(string name, Func<TValue> getValue, IEqualityComparer<TValue> comparer)
    {
        this.Name = name;
        this._getValue = getValue;
        this._comparer = comparer;
        this.CurrentValue = getValue();
        this.PreviousValue = this.CurrentValue;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public TValue PreviousValue { get; private set; }

    /// <inheritdoc />
    public TValue CurrentValue { get; private set; }

    /// <inheritdoc />
    public bool IsChanged { get; private set; }

    /// <inheritdoc />
    public void Update()
    {
        this.CurrentValue = this._getValue();
        this.IsChanged = !this._comparer.Equals(this.PreviousValue, this.CurrentValue);
    }

    /// <inheritdoc />
    public void Reset()
    {
        this.PreviousValue = this.CurrentValue;
        this.IsChanged = false;
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
