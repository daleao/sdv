namespace DaLion.Chargeable.Framework.Integrations;

#region using directives

using System.Collections.Generic;
using Microsoft.Xna.Framework;

#endregion using directives

public interface IItemExtensionsApi
{
    bool IsStone(string id);

    bool IsResource(string id, out int? health, out string itemDropped);

    bool HasBehavior(string qualifiedItemId, string target);

    bool IsClump(string qualifiedItemId);

    bool TrySpawnClump(string itemId, Vector2 position, string locationName, out string error, bool avoidOverlap = false);

    bool TrySpawnClump(string itemId, Vector2 position, GameLocation location, out string error, bool avoidOverlap = false);

    List<string> GetCustomSeeds(string itemId, bool includeSource, bool parseConditions = true);
}
