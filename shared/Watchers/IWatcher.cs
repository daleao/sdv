namespace DaLion.Shared.Watchers;

/// <summary>A watcher which detects changes to something.</summary>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal interface IWatcher : IDisposable
{
    /// <summary>Gets a name which identifies what the watcher is watching, used for troubleshooting.</summary>
    string Name { get; }

    /// <summary>Gets a value indicating whether the watched value changed since the last reset.</summary>
    bool IsChanged { get; }

    /// <summary>Update the current value if needed.</summary>
    void Update();

    /// <summary>Set the current value as the baseline.</summary>
    void Reset();
}
