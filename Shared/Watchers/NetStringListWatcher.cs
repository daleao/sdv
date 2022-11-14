namespace DaLion.Shared.Watchers;

#region using directives

using System.Collections.Generic;
using DaLion.Shared.Comparers;
using Netcode;

#endregion using directives

/// <summary>A watcher which detects changes to a net list field.</summary>
internal class NetStringListWatcher : DisposableWatcher, ICollectionWatcher<string>
{
    /// <summary>The field being watched.</summary>
    private readonly NetList<string, NetString> _field;

    /// <summary>The pairs added since the last reset.</summary>
    private readonly ISet<string> _added = new HashSet<string>(new EquatableComparer<string>());

    /// <summary>The pairs removed since the last reset.</summary>
    private readonly ISet<string> _removed = new HashSet<string>(new EquatableComparer<string>());

    /// <summary>Initializes a new instance of the <see cref="NetStringListWatcher"/> class.</summary>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="field">The field to watch.</param>
    public NetStringListWatcher(string name, NetList<string, NetString> field)
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
    public IEnumerable<string> Added => this._added;

    /// <inheritdoc />
    public IEnumerable<string> Removed => this._removed;

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
    private void OnArrayReplaced(NetList<string, NetString> list, IList<string> oldValues, IList<string> newValues)
    {
        ISet<string> oldSet = new HashSet<string>(oldValues, new EquatableComparer<string>());
        ISet<string> changed = new HashSet<string>(newValues, new EquatableComparer<string>());

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
    private void OnElementChanged(NetList<string, NetString> list, int index, string oldValue, string newValue)
    {
        this.Remove(oldValue);
        this.Add(newValue);
    }

    /// <summary>Track an added item.</summary>
    /// <param name="value">The value that was added.</param>
    private void Add(string value)
    {
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
    private void Remove(string value)
    {
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
