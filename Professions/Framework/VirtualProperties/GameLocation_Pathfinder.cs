namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Pathfinding;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GameLocation_Pathfinder
{
    internal static ConditionalWeakTable<GameLocation, AStar> Values { get; } = [];

    internal static AStar Get_Pathfinder(this GameLocation location)
    {
        return Values.GetValue(location, Create);
    }

    private static AStar Create(GameLocation location)
    {
        var collisionMask = CollisionMask.Buildings | CollisionMask.Furniture | CollisionMask.Objects |
                            CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific;
        return new AStar(
            location,
            (l, t) => l.isTilePassable(t) && !l.IsTileOccupiedBy(t, collisionMask));
    }
}
