namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Step
{
    internal static ConditionalWeakTable<GreenSlime, Holder> Values { get; } = [];

    internal static Point Get_Step(this GreenSlime slime)
    {
        return Values.GetOrCreateValue(slime).Step;
    }

    internal static void Set_Step(this GreenSlime slime, Point value)
    {
        Values.GetOrCreateValue(slime).Step = value;
    }

    internal class Holder
    {
        public Point Step { get; internal set; }
    }
}
