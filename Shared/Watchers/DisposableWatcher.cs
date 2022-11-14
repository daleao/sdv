namespace DaLion.Shared.Watchers;

/// <summary>The base implementation for a disposable watcher.</summary>
/// <remarks>Pulled from <see href="https://github.com/Pathoschild/SMAPI/tree/develop/src/SMAPI/Modules/StateTracking">SMAPI</see>.</remarks>
internal abstract class DisposableWatcher : IDisposable
{
    /// <summary>Gets a value indicating whether the watcher has been disposed.</summary>
    protected bool IsDisposed { get; private set; }

    /// <summary>Stop watching the field and release all references.</summary>
    public virtual void Dispose()
    {
        this.IsDisposed = true;
    }

    /// <summary>Throw an exception if the watcher is disposed.</summary>
    /// <exception cref="ObjectDisposedException">The watcher is disposed.</exception>
    protected void AssertNotDisposed()
    {
        if (this.IsDisposed)
        {
            ThrowHelper.ThrowObjectDisposedException(this.GetType().Name);
        }
    }
}
