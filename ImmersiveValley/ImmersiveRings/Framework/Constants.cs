namespace DaLion.Stardew.Rings.Framework;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

internal class Constants
{
    internal const int GLOW_RING_INDEX_I = 517,
        MAGNET_RING_INDEX_I = 519,
        IRIDIUM_BAND_INDEX_I = 527,
        AMETHYSTR_RING_INDEX_I = 529,
        TOPAZ_RING_INDEX_I = 530,
        AQUAMARINE_RING_INDEX_I = 531,
        JADE_RING_INDEX_I = 532,
        EMERALD_RING_INDEX_I = 533,
        RUBY_RING_INDEX_I = 534,
        CRAB_RING_INDEX_I = 810;

    internal static readonly Dictionary<int, Color> ColorByGemstone = new()
    {
        {AMETHYSTR_RING_INDEX_I, new(111, 60, 196)},
        {TOPAZ_RING_INDEX_I, new(220, 143, 8)},
        {AQUAMARINE_RING_INDEX_I, new(35, 144, 170)},
        {JADE_RING_INDEX_I, new(117, 150, 99)},
        {EMERALD_RING_INDEX_I, new(4, 128, 54)},
        {RUBY_RING_INDEX_I, new(225, 57, 57)}
    };
}