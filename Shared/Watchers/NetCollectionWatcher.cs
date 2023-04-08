namespace DaLion.Shared.Watchers;

#region using directives

using System.Collections.Generic;
using Netcode;

#endregion using directives

/// <summary>A watcher which detects changes to a <see cref="NetCollection{T}"/>.</summary>
/// <typeparam name="TValue">The value type within the collection.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal class NetCollectionWatcher<TValue> : DisposableWatcher, ICollectionWatcher<TValue>
    where TValue : class, INetObject<INetSerializable>
{
    /// <summary>The field being watched.</summary>
    private readonly NetCollection<TValue> _field;

    /// <summary>The pairs added since the last reset.</summary>
    private readonly List<TValue> _added = new();

    /// <summary>The pairs removed since the last reset.</summary>
    private readonly List<TValue> _removed = new();

    /// <summary>Initializes a new instance of the <see cref="NetCollectionWatcher{TValue}"/> class.</summary>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="field">The field to watch.</param>
    public NetCollectionWatcher(string name, NetCollection<TValue> field)
    {
        this.Name = name;
        this._field = field;
        field.OnValueAdded += this.OnValueAdded;
        field.OnValueRemoved += this.OnValueRemoved;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public bool IsChanged => this._added.Count > 0 || this._removed.Count > 0;

    /// <inheritdoc />
    public IEnumerable<TValue> Added => this._added;

    /// <inheritdoc />
    public IEnumerable<TValue> Removed => this._removed;

    /// <inheritdoc />
    public void Update()
    {
        this.AssertNotDisposed();
    }

    /// <inheritdoc />
    public void Reset()
    {
        this.AssertNotDisposed();

        this._added.Clear();
        this._removed.Clear();
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        if (!this.IsDisposed)
        {
            this._field.OnValueAdded -= this.OnValueAdded;
            this._field.OnValueRemoved -= this.OnValueRemoved;
        }

        base.Dispose();
    }

    /// <summary>A callback invoked when an entry is added to the collection.</summary>
    /// <param name="value">The added value.</param>
    private void OnValueAdded(TValue value)
    {
        this._added.Add(value);
    }

    /// <summary>A callback invoked when an entry is removed from the collection.</summary>
    /// <param name="value">The added value.</param>
    private void OnValueRemoved(TValue value)
    {
        this._removed.Add(value);
    }
}
