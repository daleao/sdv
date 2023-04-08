namespace DaLion.Shared.Watchers;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Comparers;
using Netcode;

#endregion using directives

/// <summary>A watcher which detects changes to a <see cref="NetList{T,TField}"/>.</summary>
/// <typeparam name="TValue">The list value type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal class NetListWatcher<TValue> : DisposableWatcher, ICollectionWatcher<TValue>
    where TValue : class, INetObject<INetSerializable>
{
    /// <summary>The field being watched.</summary>
    private readonly NetList<TValue, NetRef<TValue>> _field;

    /// <summary>The pairs added since the last reset.</summary>
    private readonly HashSet<TValue> _added = new(new ObjectReferenceComparer<TValue>());

    /// <summary>The pairs removed since the last reset.</summary>
    private readonly HashSet<TValue> _removed = new(new ObjectReferenceComparer<TValue>());

    /// <summary>Initializes a new instance of the <see cref="NetListWatcher{TValue}"/> class.</summary>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="field">The field to watch.</param>
    public NetListWatcher(string name, NetList<TValue, NetRef<TValue>> field)
    {
        this.Name = name;
        this._field = field;
        field.OnArrayReplaced += this.OnArrayReplaced;
        field.OnElementChanged += this.OnElementChanged;
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
    public void Reset()
    {
        this._added.Clear();
        this._removed.Clear();
    }

    /// <inheritdoc />
    public void Update()
    {
        this.AssertNotDisposed();
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        if (!this.IsDisposed)
        {
            this._field.OnArrayReplaced -= this.OnArrayReplaced;
            this._field.OnElementChanged -= this.OnElementChanged;
        }

        base.Dispose();
    }

    /// <summary>A callback invoked when the value list is replaced.</summary>
    /// <param name="list">The net field whose values changed.</param>
    /// <param name="oldValues">The previous list of values.</param>
    /// <param name="newValues">The new list of values.</param>
    private void OnArrayReplaced(NetList<TValue, NetRef<TValue>> list, IList<TValue> oldValues, IList<TValue> newValues)
    {
        var oldSet = new HashSet<TValue>(oldValues, new ObjectReferenceComparer<TValue>());
        var changed = new HashSet<TValue>(newValues, new ObjectReferenceComparer<TValue>());

        foreach (var value in oldSet)
        {
            if (!changed.Contains(value))
            {
                this.Remove(value);
            }
        }

        foreach (var value in changed)
        {
            if (!oldSet.Contains(value))
            {
                this.Add(value);
            }
        }
    }

    /// <summary>A callback invoked when an entry is replaced.</summary>
    /// <param name="list">The net field whose values changed.</param>
    /// <param name="index">The list index which changed.</param>
    /// <param name="oldValue">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private void OnElementChanged(NetList<TValue, NetRef<TValue>> list, int index, TValue? oldValue, TValue? newValue)
    {
        this.Remove(oldValue);
        this.Add(newValue);
    }

    /// <summary>Track an added item.</summary>
    /// <param name="value">The value that was added.</param>
    private void Add(TValue? value)
    {
        if (value == null)
        {
            return;
        }

        if (this._removed.Contains(value))
        {
            this._added.Remove(value);
            this._removed.Remove(value);
        }
        else
        {
            this._added.Add(value);
        }
    }

    /// <summary>Track a removed item.</summary>
    /// <param name="value">The value that was removed.</param>
    private void Remove(TValue? value)
    {
        if (value == null)
        {
            return;
        }

        if (this._added.Contains(value))
        {
            this._added.Remove(value);
            this._removed.Remove(value);
        }
        else
        {
            this._removed.Add(value);
        }
    }
}
