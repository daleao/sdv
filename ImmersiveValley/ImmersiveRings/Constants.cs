namespace DaLion.Stardew.Rings;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

public class Constants
{
    public const int 
        SMALL_GLOW_RING_INDEX_I = 516,
        GLOW_RING_INDEX_I = 517,
        SMALL_MAGNET_RING_INDEX_I = 518,
        MAGNET_RING_INDEX_I = 519,
        IRIDIUM_BAND_INDEX_I = 527,
        AMETHYSTR_RING_INDEX_I = 529,
        TOPAZ_RING_INDEX_I = 530,
        AQUAMARINE_RING_INDEX_I = 531,
        JADE_RING_INDEX_I = 532,
        EMERALD_RING_INDEX_I = 533,
        RUBY_RING_INDEX_I = 534,
        SUN_ESSENCE_INDEX_I = 768,
        VOID_ESSENCE_INDEX_I = 769,
        CRAB_RING_INDEX_I = 810,
        GLOWSTONE_RING_INDEX_I = 888;

    public static readonly Dictionary<int, Color> ColorByGemstone = new()
    {
        {AMETHYSTR_RING_INDEX_I, new(111, 60, 196)},
        {TOPAZ_RING_INDEX_I, new(220, 143, 8)},
        {AQUAMARINE_RING_INDEX_I, new(35, 144, 170)},
        {JADE_RING_INDEX_I, new(117, 150, 99)},
        {EMERALD_RING_INDEX_I, new(4, 128, 54)},
        {RUBY_RING_INDEX_I, new(225, 57, 57)}
    };
}