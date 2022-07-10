namespace DaLion.Stardew.Rings;

#region using directives

using Microsoft.Xna.Framework;
using System.Collections.Generic;

#endregion using directives

internal static class Utils
{
    /// <summary>Get the gemstone of the corresponding ring.</summary>
    public static readonly Dictionary<int, int> GemstoneByRing = new()
    {
        { Constants.AMETHYST_RING_INDEX_I, Constants.AMETHYST_INDEX_I },
        { Constants.TOPAZ_RING_INDEX_I, Constants.TOPAZ_INDEX_I },
        { Constants.AQUAMARINE_RING_INDEX_I, Constants.AQUAMARINE_INDEX_I },
        { Constants.JADE_RING_INDEX_I, Constants.JADE_INDEX_I },
        { Constants.EMERALD_RING_INDEX_I, Constants.EMERALD_INDEX_I },
        { Constants.RUBY_RING_INDEX_I, Constants.RUBY_INDEX_I }
    };

    /// <summary>Get the color of the corresponding gemstone.</summary>
    public static readonly Dictionary<int, Color> ColorByGemstone = new()
    {
        { Constants.AMETHYST_RING_INDEX_I, new(111, 60, 196) },
        { Constants.TOPAZ_RING_INDEX_I, new(220, 143, 8) },
        { Constants.AQUAMARINE_RING_INDEX_I, new(35, 144, 170) },
        { Constants.JADE_RING_INDEX_I, new(117, 150, 99) },
        { Constants.EMERALD_RING_INDEX_I, new(4, 128, 54) },
        { Constants.RUBY_RING_INDEX_I, new(225, 57, 57) }
    };
}