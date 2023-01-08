using System;

namespace DaLion.Meads;

internal class Globals
{
    public const int MEAD_INDEX_I = 459;

    public static readonly object MeadAsArtisanGoodEnum =
        Enum.ToObject("BetterArtisanGoodIcons.ArtisanGood".ToType(), MEAD_INDEX_I);
}