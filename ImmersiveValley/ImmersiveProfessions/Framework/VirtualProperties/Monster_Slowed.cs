namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using Netcode;
using StardewValley.Monsters;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Monster_Slowed
{
    internal class Holder
    {
        public readonly NetInt slowTimer = new(-1);
        public readonly NetInt slowIntensity = new(-1);
        public Farmer slower = null!;
    }

    internal static ConditionalWeakTable<Monster, Holder> Values = new();

    public static NetInt get_SlowTimer(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.slowTimer;
    }

    // Net types are readonly
    public static void set_SlowTmer(this Monster monster, NetInt newVal) { }

    public static NetInt get_SlowIntensity(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.slowIntensity;
    }

    // Net types are readonly
    public static void set_SlowIntensity(this Monster monster, NetInt newVal) { }

    public static Farmer get_Slower(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.slower;
    }

    public static void set_Slower(this Monster monster, Farmer slower)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.slower = slower;
    }
}