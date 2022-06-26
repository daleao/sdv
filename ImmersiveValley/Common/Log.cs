namespace DaLion.Common;

#region using directives

using System.Diagnostics;
using StardewModdingAPI;

#endregion using directives

/// <summary>Simplified wrapper for SMAPI's <see cref="IMonitor"/>.</summary>
public static class Log
{
    /// <inheritdoc cref="IMonitor"/>
    private static IMonitor _Monitor = null!;

    /// <summary>Initialize static instance.</summary>
    /// <param name="monitor">Encapsulates monitoring and logging for a given module.</param>
    public static void Init(IMonitor monitor)
    {
        _Monitor = monitor;
    }

    /// <summary>Log a message as debug.</summary>
    /// <param name="message">The message.</param>
    [Conditional("DEBUG")]
    public static void D(string message)
    {
        _Monitor.Log(message, LogLevel.Debug);
    }

    /// <summary>Log a message as trace.</summary>
    /// <param name="message">The message.</param>
    public static void T(string message)
    {
        _Monitor.Log(message, LogLevel.Trace);
    }

    /// <summary>Log a message as info.</summary>
    /// <param name="message">The message.</param>
    public static void I(string message)
    {
        _Monitor.Log(message, LogLevel.Info);
    }

    /// <summary>Log a message as alert.</summary>
    /// <param name="message">The message.</param>
    public static void A(string message)
    {
        _Monitor.Log(message, LogLevel.Alert);
    }

    /// <summary>Log a message as warn.</summary>
    /// <param name="message">The message.</param>
    public static void W(string message)
    {
        _Monitor.Log(message, LogLevel.Warn);
    }

    /// <summary>Log a message as error.</summary>
    /// <param name="message">The message.</param>
    public static void E(string message)
    {
        _Monitor.Log(message, LogLevel.Error);
    }

    /// <inheritdoc cref="IMonitor.VerboseLog"/>
    public static void V(string message)
    {
        _Monitor.VerboseLog(message);
    }
}