namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using StardewValley;
using StardewValley.Monsters;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Monster_Taunted
{
    internal class Holder
    {
        public Character? taunter;
        public Farmer? fakeFarmer;
    }

    internal static ConditionalWeakTable<Monster, Holder> Values = new();

    public static Character? get_Taunter(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.taunter;
    }

    public static void set_Taunter(this Monster monster, Character? taunter)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.taunter = taunter;
        holder.fakeFarmer = new()
        { UniqueMultiplayerID = monster.GetHashCode(), currentLocation = monster.currentLocation };
    }

    public static Farmer? get_FakeFarmer(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.fakeFarmer;
    }
}