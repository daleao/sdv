namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using StardewValley.Monsters;
using StardewValley.Network;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Monster_Taunted
{
    internal class Holder
    {
        public NetCharacterRef taunter = new();
        public Farmer? fakeFarmer;
    }

    internal static ConditionalWeakTable<Monster, Holder> Values = new();

    public static NetCharacterRef get_Taunter(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.taunter;
    }

    public static void set_Taunter(this Monster monster, Character? taunter)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.taunter.Set(taunter?.currentLocation, taunter);
        holder.fakeFarmer = taunter is null ? null : new()
        { UniqueMultiplayerID = monster.GetHashCode(), currentLocation = monster.currentLocation };
    }

    public static Farmer? get_FakeFarmer(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.fakeFarmer;
    }
}