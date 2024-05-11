namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Classes;
using Microsoft.Xna.Framework;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GameLocation_Quadtree
{
    internal static ConditionalWeakTable<GameLocation, Quadtree<Character>> Values { get; } = [];

    // returns nullable to avoid unnecessary iteration
    internal static Quadtree<Character> Get_Quadtree(this GameLocation location)
    {
        return Values.GetValue(location, Create);
    }

    private static Quadtree<Character> Create(GameLocation location)
    {
        var regionBounds = new Rectangle(0, 0, location.Map.DisplayWidth, location.Map.DisplayHeight);
        var tree = new Quadtree<Character>(regionBounds, c => c.GetBoundingBox());
        foreach (var c in location.characters)
        {
            tree.Insert(c);
        }

        return tree;
    }
}
