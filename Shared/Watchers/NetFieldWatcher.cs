namespace DaLion.Shared.Watchers;

#region using directives

using Netcode;

#endregion using directives

/// <summary>A watcher which detects changes to a net value field.</summary>
/// <typeparam name="TValue">The value type wrapped by the net field.</typeparam>
/// <typeparam name="TNetField">The net field type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal class NetFieldWatcher<TValue, TNetField> : DisposableWatcher, IValueWatcher<TValue>
    where TNetField : NetFieldBase<TValue, TNetField>
{
    /// <summary>The field being watched.</summary>
    private readonly NetFieldBase<TValue, TNetField> _field;

    /// <summary>Initializes a new instance of the <see cref="NetFieldWatcher{TValue,TNetField}"/> class.</summary>
    /// <param name="name">A name which identifies what the watcher is watching, used for troubleshooting.</param>
    /// <param name="field">The field to watch.</param>
    public NetFieldWatcher(string name, NetFieldBase<TValue, TNetField> field)
    {
        this.Name = name;
        this._field = field;
        this.PreviousValue = field.Value;
        this.CurrentValue = field.Value;

        field.fieldChangeVisibleEvent += this.OnValueChanged;
        field.fieldChangeEvent += this.OnValueChanged;
    }

    /// <inheritdoc />
    public string Name { get; }

    /// <inheritdoc />
    public bool IsChanged { get; private set; }

    /// <inheritdoc />
    public TValue PreviousValue { get; private set; }

    /// <inheritdoc />
    public TValue CurrentValue { get; private set; }

    /// <inheritdoc />
    public void Update()
    {
        this.AssertNotDisposed();
    }

    /// <inheritdoc />
    public void Reset()
    {
        this.AssertNotDisposed();

        this.PreviousValue = this.CurrentValue;
        this.IsChanged = false;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        if (!this.IsDisposed)
        {
            this._field.fieldChangeEvent -= this.OnValueChanged;
            this._field.fieldChangeVisibleEvent -= this.OnValueChanged;
        }

        base.Dispose();
    }

    /// <summary>A callback invoked when the field's value changes.</summary>
    /// <param name="field">The field being watched.</param>
    /// <param name="oldValue">The old field value.</param>
    /// <param name="newValue">The new field value.</param>
    private void OnValueChanged(TNetField field, TValue oldValue, TValue newValue)
    {
        this.CurrentValue = newValue;
        this.IsChanged = true;
    }
}
