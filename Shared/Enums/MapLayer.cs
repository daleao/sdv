namespace DaLion.Shared.Enums;

/// <summary>A <see cref="xTile.Layers.Layer"/> within in a <see cref="xTile.Map"/>.</summary>
public enum MapLayer
{
    /// <summary>The back layer, representing walkable ground or floor tiles.</summary>
    Back,

    /// <summary>The buildings layer, representing walls and obstacles.</summary>
    Buildings,

    /// <summary>The front layer, which renders above characters. Commonly used for adding detail over buildings or hiding invisible walls.</summary>
    Front,
}
