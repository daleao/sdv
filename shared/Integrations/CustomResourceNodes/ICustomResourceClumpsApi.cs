#pragma warning disable CS1591
namespace DaLion.Shared.Integrations.CustomResourceClumps;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley.TerrainFeatures;

#endregion using directives

/// <summary>The API provided by Custom Ore Nodes.</summary>
public interface ICustomResourceClumpsApi
{
    ResourceClump GetCustomClump(string id, Vector2 tile);

    bool TryPlaceClump(GameLocation location, string id, Vector2 tile);

    List<object> GetCustomClumpData();

    List<string> GetCustomClumpIDs();
}
