namespace DaLion.Redux.Framework;

/// <summary>Holds global variables that may be used by different modules.</summary>
internal sealed class Globals
{
    /// <summary>Gets the <see cref="Professions.HudPointer"/> which points at various points of interest.</summary>
    internal static Lazy<Professions.HudPointer> Pointer { get; } = new(() => new Professions.HudPointer());

    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet gemstone (provided by Json Assets).</summary>
    internal static int GarnetIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet Ring (provided by Json Assets).</summary>
    internal static int GarnetRingIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Infinity Band (provided by Json Assets).</summary>
    internal static int InfinityBandIndex { get; set; }

    /// <summary>Gets or sets the <see cref="FrameRateCounter"/>.</summary>
    internal static FrameRateCounter? FpsCounter { get; set; }

    /// <summary>Gets or sets the latest position of the cursor.</summary>
    internal static ICursorPosition? DebugCursorPosition { get; set; }
}
