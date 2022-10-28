namespace DaLion.Common.Integrations.ImmersiveRings;

#region using directives

using StardewValley.Objects;

#endregion using directives

/// <summary>Implementation of the mod API.</summary>
public interface IImmersiveRingsApi
{
    /// <summary>Gets the <see cref="IImmersiveRings.IChord"/> for the specified <paramref name="ring"/>, if any.</summary>
    /// <param name="ring">A <see cref="CombinedRing"/> which possibly contains a <see cref="IImmersiveRings.IChord"/>.</param>
    /// <returns>The <see cref="IImmersiveRings.IChord"/> instance if the <paramref name="ring"/> is an Infinity Band with at least two gemstone, otherwise <see langword="null"/>.</returns>
    IImmersiveRings.IChord? GetChord(CombinedRing ring);
}
