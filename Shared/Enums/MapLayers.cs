namespace DaLion.Shared.Enums;

#region using directives

using xTile;

#endregion using directives

/// <summary>A Tiled <see cref="xTile.Map"> <see cref="xTile.Layers.Layer"/>.</summary>
public enum MapLayers
{
    /// <summary>The back layer, which contains walkable ground and floor tiles.</summary>
    Back,

    /// <summary>The buildings layer, which contains walls and obstacles.</summary>
    Buildings,

    /// <summary>The front layer, which renders above characters. Often used to add detail or hide hidden walls.</summary>
    Front,
}
