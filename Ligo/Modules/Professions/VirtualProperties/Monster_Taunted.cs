namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;
using StardewValley.Network;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Taunted
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static Monster? Get_Taunter(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.Taunter;
    }

    internal static void Set_Taunter(this Monster monster, Monster? taunter)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.Taunter = taunter;
        holder.FakeFarmer = taunter is null
            ? null
            : new FakeFarmer { UniqueMultiplayerID = monster.GetHashCode(), currentLocation = monster.currentLocation };
    }

    internal static Farmer? Get_FakeFarmer(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.FakeFarmer;
    }

    internal class Holder
    {
        public Monster? Taunter { get; internal set; }

        public FakeFarmer? FakeFarmer { get; internal set; }
    }
}
