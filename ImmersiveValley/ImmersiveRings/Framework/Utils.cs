namespace DaLion.Stardew.Rings;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

internal static class Utils
{
    /// <summary>Get the gemstone of the corresponding ring.</summary>
    public static readonly Dictionary<int, int> GemstoneByRing = new()
    {
        { Constants.AMETHYSTR_RING_INDEX_I, Constants.AMETHYST_INDEX_I },
        { Constants.TOPAZ_RING_INDEX_I, Constants.TOPAZ_INDEX_I },
        { Constants.AQUAMARINE_RING_INDEX_I, Constants.AQUAMARINE_INDEX_I },
        { Constants.JADE_RING_INDEX_I, Constants.JADE_INDEX_I },
        { Constants.EMERALD_RING_INDEX_I, Constants.EMERALD_INDEX_I },
        { Constants.RUBY_RING_INDEX_I, Constants.RUBY_INDEX_I }
    };

    /// <summary>Get the color of the corresponding gemstone.</summary>
    public static readonly Dictionary<int, Color> ColorByGemstone = new()
    {
        { Constants.AMETHYSTR_RING_INDEX_I, new(111, 60, 196) },
        { Constants.TOPAZ_RING_INDEX_I, new(220, 143, 8) },
        { Constants.AQUAMARINE_RING_INDEX_I, new(35, 144, 170) },
        { Constants.JADE_RING_INDEX_I, new(117, 150, 99) },
        { Constants.EMERALD_RING_INDEX_I, new(4, 128, 54) },
        { Constants.RUBY_RING_INDEX_I, new(225, 57, 57) }
    };

    /// <summary>Get the source rectangle coordinates of the corresponding gemstone.</summary>
    /// <remarks>Used for Better Rings texture.</remarks>
    public static readonly Dictionary<int, Rectangle> SourceRectByGemstone = new()
    {
        {Constants.AMETHYSTR_RING_INDEX_I, new(0, 0, 16, 16)},
        {Constants.TOPAZ_RING_INDEX_I, new(16, 0, 16, 16)},
        {Constants.AQUAMARINE_RING_INDEX_I, new(32, 0, 16, 16)},
        {Constants.JADE_RING_INDEX_I, new(48, 0, 16, 16)},
        {Constants.EMERALD_RING_INDEX_I, new(64, 0, 16, 16)},
        {Constants.RUBY_RING_INDEX_I, new(80, 0, 16, 16)}
    };
}