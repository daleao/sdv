namespace DaLion.Stardew.Rings;

#region using directives

using DaLion.Stardew.Rings.Framework.Resonance;
using DaLion.Stardew.Rings.Framework.VirtualProperties;
using StardewValley.Objects;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public sealed class ModApi
{
    /// <summary>Gets the <see cref="IChord"/> for the specified <paramref name="ring"/>, if any.</summary>
    /// <param name="ring">A <see cref="CombinedRing"/> which possibly contains a <see cref="IChord"/>.</param>
    /// <returns>The <see cref="IChord"/> instance if the <paramref name="ring"/> is an Infinity Band with at least two gemstone, otherwise <see langword="null"/>.</returns>
    public IChord? GetChord(CombinedRing ring)
    {
        return ring.Get_Chord();
    }
}
