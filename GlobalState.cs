namespace DaLion.Overhaul;

/// <summary>Holds global variables that may be used by different modules.</summary>
internal sealed class GlobalState
{
    internal static bool AreEnemiesAround { get; set; }

    /// <summary>Gets or sets the number of elapsed seconds since the last combat-related action.</summary>
    internal static int SecondsOutOfCombat { get; set; }

    /// <summary>Gets or sets the <see cref="FrameRateCounter"/>.</summary>
    internal static FrameRateCounter? FpsCounter { get; set; }

    /// <summary>Gets or sets the latest position of the cursor.</summary>
    internal static ICursorPosition? DebugCursorPosition { get; set; }
}
