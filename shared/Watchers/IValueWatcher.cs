namespace DaLion.Shared.Watchers;

/// <summary>A watcher which tracks changes to a value.</summary>
/// <typeparam name="TValue">The watched value type.</typeparam>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal interface IValueWatcher<out TValue> : IWatcher
{
    /// <summary>Gets the field value at the last reset.</summary>
    TValue PreviousValue { get; }

    /// <summary>Gets the latest value.</summary>
    TValue CurrentValue { get; }
}
