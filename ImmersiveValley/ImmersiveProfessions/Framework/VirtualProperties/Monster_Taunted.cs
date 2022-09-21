namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;
using StardewValley.Network;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Taunted
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetCharacterRef Get_Taunter(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.Taunter;
    }

    internal static void Set_Taunter(this Monster monster, Character? taunter)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.Taunter.Set(taunter?.currentLocation, taunter);
        holder.FakeFarmer = taunter is null
            ? null
            : new Farmer { UniqueMultiplayerID = monster.GetHashCode(), currentLocation = monster.currentLocation };
    }

    internal static Farmer? Get_FakeFarmer(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.FakeFarmer;
    }

    internal class Holder
    {
        public NetCharacterRef Taunter { get; } = new();

        public Farmer? FakeFarmer { get; set; }
    }
}
