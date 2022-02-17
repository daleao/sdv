using System.Diagnostics;

namespace DaLion.Stardew.Professions;

#region using directives

using StardewModdingAPI;

#endregion using directives

/// <summary>Wrapper for SMAPI's <see cref="IMonitor.Log"/>.</summary>
public static class Log
{
    /// <summary>Log a message as alert.</summary>
    /// <param name="message">The message.</param>
    public static void A(string message)
    {
        ModEntry.Log(message, LogLevel.Alert);
    }

    /// <summary>Log a message as debug.</summary>
    /// <param name="message">The message.</param>
    [Conditional("DEBUG")]
    public static void D(string message)
    {
        ModEntry.Log(message, LogLevel.Debug);
    }

    /// <summary>Log a message as error.</summary>
    /// <param name="message">The message.</param>
    public static void E(string message)
    {
        ModEntry.Log(message, LogLevel.Error);
    }

    /// <summary>Log a message as info.</summary>
    /// <param name="message">The message.</param>
    public static void I(string message)
    {
        ModEntry.Log(message, LogLevel.Info);
    }

    /// <summary>Log a message as trace.</summary>
    /// <param name="message">The message.</param>
    public static void T(string message)
    {
        ModEntry.Log(message, LogLevel.Trace);
    }

    /// <summary>Log a message as warn.</summary>
    /// <param name="message">The message.</param>
    public static void W(string message)
    {
        ModEntry.Log(message, LogLevel.Warn);
    }
}