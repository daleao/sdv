namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Pathfinding;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Pathfinder
{
    internal static ConditionalWeakTable<GreenSlime, DStarLite> Values { get; } = [];

    internal static DStarLite Get_Pathfinder(this GreenSlime slime)
    {
        return Values.GetValue(slime, Create);
    }

    private static DStarLite Create(GreenSlime slime)
    {
        var collisionMask = CollisionMask.Buildings | CollisionMask.Furniture | CollisionMask.Objects |
                            CollisionMask.TerrainFeatures | CollisionMask.LocationSpecific;
        return new DStarLite(
            slime.currentLocation,
            (l, t) => l.isTilePassable(t) && !l.IsTileOccupiedBy(t, collisionMask));
    }
}
