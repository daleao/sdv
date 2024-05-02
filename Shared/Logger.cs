namespace DaLion.Shared;

#region using directives

using System.Diagnostics;

#endregion using directives

/// <summary>Simplified wrapper for SMAPI's <see cref="IMonitor"/>.</summary>
/// <remarks>Initializes the static instance.</remarks>
/// <param name="monitor">Encapsulates monitoring and logging for a given module.</param>
public sealed class Logger(IMonitor monitor)
{
    /// <inheritdoc cref="IMonitor"/>
    private readonly IMonitor _monitor = monitor;

    /// <summary>Logs a message as debug.</summary>
    /// <param name="message">The message.</param>
    [Conditional("DEBUG")]
    public void D(string message)
    {
        this._monitor.Log(message, LogLevel.Debug);
    }

    /// <summary>Logs a message as trace.</summary>
    /// <param name="message">The message.</param>
    public void T(string message)
    {
#if DEBUG
        this.D(message);
#else
        this._monitor.Log(message);
#endif
    }

    /// <summary>Logs a message as info.</summary>
    /// <param name="message">The message.</param>
    public void I(string message)
    {
        this._monitor.Log(message, LogLevel.Info);
    }

    /// <summary>Logs a message as alert.</summary>
    /// <param name="message">The message.</param>
    public void A(string message)
    {
        this._monitor.Log(message, LogLevel.Alert);
    }

    /// <summary>Logs a message as warn.</summary>
    /// <param name="message">The message.</param>
    public void W(string message)
    {
        this._monitor.Log(message, LogLevel.Warn);
    }

    /// <summary>Logs a message as error.</summary>
    /// <param name="message">The message.</param>
    public void E(string message)
    {
        this._monitor.Log(message, LogLevel.Error);
    }

    /// <summary>Logs a message as error.</summary>
    /// <param name="message">The message.</param>
    public void V(string message)
    {
        this._monitor.VerboseLog(message);
    }

    /// <summary>Logs the caller method as Debug.</summary>
    [Conditional("DEBUG")]
    public void Caller()
    {
        var caller = new StackTrace().GetFrame(2)?.GetMethod()?.Name ?? string.Empty;
        this.D($"Called by {caller}.");
    }

    /// <summary>Logs the entire stack trace.</summary>
    [Conditional("DEBUG")]
    public void StackTrace()
    {
        this.D(Environment.StackTrace);
    }
}
