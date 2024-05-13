namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Path
{
    internal static ConditionalWeakTable<GreenSlime, Stack<Point>> Values { get; } = [];

    internal static Stack<Point> Get_Path(this GreenSlime slime)
    {
        return Values.GetOrCreateValue(slime);
    }

    internal static void Set_Path(this GreenSlime slime, Stack<Point> value)
    {
        Values.AddOrUpdate(slime, value);
    }
}
