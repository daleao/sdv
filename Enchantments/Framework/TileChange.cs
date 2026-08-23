namespace DaLion.Enchantments.Framework;

#region using directives

using DaLion.Shared.Enums;

#endregion using directive

/// <summary>Represents a single changed tile resulting from an excavated wall tile.</summary>
/// <param name="Layer">The <see cref="MapLayer"/>.</param>
/// <param name="X">The x-coordinate relative to the excavated tile.</param>
/// <param name="Y">The y-coordinate relative to the excavated tile.</param>
/// <param name="TileIndex">The index of the new sprite in the tilesheet.</param>
internal sealed record TileChange(MapLayer Layer, int X, int Y, int TileIndex);
