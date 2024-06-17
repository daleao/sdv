namespace DaLion.Core.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
public static class Monster_Target
{
    internal static ConditionalWeakTable<Monster, Farmer?> Values { get; } = [];

    public static Farmer Get_Target(this Monster monster)
    {
        return Values.GetOrCreateValue(monster) ?? Game1.player;
    }

    public static void Set_Target(this Monster monster, Farmer? target)
    {
        Values.AddOrUpdate(monster, target);
    }
}
