namespace DaLion.Shared.Watchers;

#region using directives

using System.Collections.Generic;
using Netcode;

#endregion using directives

/// <summary>A watcher which detects changes to a net dictionary field.</summary>
/// <typeparam name="TKey">The dictionary key type.</typeparam>
/// <typeparam name="TValue">The dictionary value type.</typeparam>
/// <typeparam name="TField">The net type equivalent to <typeparamref name="TValue"/>.</typeparam>
/// <typeparam name="TSerialDict">The serializable dictionary type that can store the keys and values.</typeparam>
/// <typeparam name="TSelf">The net field instance type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal class NetDictionaryWatcher<TKey, TValue, TField, TSerialDict, TSelf> : DisposableWatcher, IDictionaryWatcher<TKey, TValue>
    where TKey : notnull
    where TField : class, INetObject<INetSerializable>, new()
    where TSerialDict : IDictionary<TKey, TValue>, new()
    where TSelf : NetDictionary<TKey, TValue, TField, TSerialDict, TSelf>
{
    /// <summary>The field being watched.</summary>
    private readonly NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> _field;

    /// <summary>The pairs added since the last reset.</summary>
    private readonly IDictionary<TKey, TValue> _added = new Dictionary<TKey, TValue>();

    /// <summary>The pairs removed since the last reset.</summary>
    private readonly IDictionary<TKey, TValue> _removed = new Dictionary<TKey, TValue>();

    /// <summary>Initializes a new instance of the <see cref="NetDictionaryWatcher{TKey, TValue, TField, TSerialDict, TSelf}"/> class.</summary>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="field">The field to watch.</param>
    public NetDictionaryWatcher(string name, NetDictionary<TKey, TValue, TField, TSerialDict, TSelf> field)
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
    public IEnumerable<KeyValuePair<TKey, TValue>> Added => this._added;

    /// <inheritdoc />
    public IEnumerable<KeyValuePair<TKey, TValue>> Removed => this._removed;

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

    /// <summary>A callback invoked when an entry is added to the dictionary.</summary>
    /// <param name="key">The entry key.</param>
    /// <param name="value">The entry value.</param>
    private void OnValueAdded(TKey key, TValue value)
    {
        this._added[key] = value;
    }

    /// <summary>A callback invoked when an entry is removed from the dictionary.</summary>
    /// <param name="key">The entry key.</param>
    /// <param name="value">The entry value.</param>
    private void OnValueRemoved(TKey key, TValue value)
    {
        if (!this._removed.ContainsKey(key))
        {
            this._removed[key] = value;
        }
    }
}
