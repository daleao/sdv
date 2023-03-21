namespace DaLion.Overhaul;

#region using directives

using DaLion.Shared.UI;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Monsters;

#endregion using directives

/// <summary>Holds global variables that may be used by different modules.</summary>
internal sealed class Globals
{
    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet gemstone (provided by Json Assets).</summary>
    internal static int? GarnetIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Garnet Ring (provided by Json Assets).</summary>
    internal static int? GarnetRingIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Infinity Band (provided by Json Assets).</summary>
    internal static int? InfinityBandIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of the Hero Soul (provided by Dynamic Game Assets).</summary>
    internal static int? HeroSoulIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of Dwarven Scrap (provided by Dynamic Game Assets).</summary>
    internal static int? DwarvenScrapIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of Elderwood (provided by Dynamic Game Assets).</summary>
    internal static int? ElderwoodIndex { get; set; }

    /// <summary>Gets or sets <see cref="Item"/> index of Dwarvish weapon blueprints (provided by Dynamic Game Assets).</summary>
    internal static int? DwarvishBlueprintIndex { get; set; }

    /// <summary>Gets or sets a value indicating whether the current location has any characters of type <see cref="Monster"/>.</summary>
    internal static bool AreEnemiesAround { get; set; }

    /// <summary>Gets or sets the <see cref="FrameRateCounter"/>.</summary>
    internal static FrameRateCounter? FpsCounter { get; set; }

    /// <summary>Gets or sets the latest position of the cursor.</summary>
    internal static ICursorPosition? DebugCursorPosition { get; set; }
}
