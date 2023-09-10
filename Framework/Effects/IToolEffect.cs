namespace DaLion.Chargeable.Framework.Effects;

#region using directives

using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>Interface for applying a <see cref="Tool"/>'s effects.</summary>
internal interface IToolEffect
{
    /// <summary>Applies the <paramref name="tool"/> to the <paramref name="tile"/>.</summary>
    /// <param name="tile">The tile to modify.</param>
    /// <param name="tileObj">The object on the tile.</param>
    /// <param name="tileFeature">The feature on the tile.</param>
    /// <param name="tool">The tool selected by the player (if any).</param>
    /// <param name="location">The current location.</param>
    /// <param name="who">The current player.</param>
    /// <returns><see langword="true"/> if the <paramref name="tool"/> was successfully applied, otherwise <see langword="false"/>.</returns>
    public bool Apply(
        Vector2 tile, SObject tileObj, TerrainFeature tileFeature, Tool tool, GameLocation location, Farmer who);
}
