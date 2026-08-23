namespace DaLion.Enchantments.Framework;

/// <summary>Represents a collection of tile changes that result from one excavated wall tile.</summary>
/// <param name="Changes">A collection of <see cref="TileChange"/>s.</param>
internal sealed record ExcavationResult(IReadOnlyList<TileChange> Changes);
