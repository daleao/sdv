namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using Netcode;
using StardewValley;
using StardewValley.Monsters;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Monster_Feared
{
    internal class Holder
    {
        public readonly NetInt fearTimer = new(-1);
        public readonly NetInt fearIntensity = new(-1);
        public Farmer fearer = null!;
    }

    internal static ConditionalWeakTable<Monster, Holder> values = new();

    public static NetInt get_FearTimer(this Monster monster)
    {
        var holder = values.GetOrCreateValue(monster);
        return holder.fearTimer;
    }

    public static void set_FearTmer(this Monster monster, NetInt newVal)
    {
        // Net types should not have a setter as they are readonly
    }

    public static NetInt get_FearIntensity(this Monster monster)
    {
        var holder = values.GetOrCreateValue(monster);
        return holder.fearIntensity;
    }

    public static void set_FearIntensity(this Monster monster, NetInt newVal)
    {
        // Net types should not have a setter as they are readonly
    }

    public static Farmer get_Fearer(this Monster monster)
    {
        var holder = values.GetOrCreateValue(monster);
        return holder.fearer;
    }

    public static void set_Fearer(this Monster monster, Farmer fearer)
    {
        var holder = values.GetOrCreateValue(monster);
        holder.fearer = fearer;
    }
}