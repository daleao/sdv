namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Taunted
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = [];

    internal static Character? Get_Taunter(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Taunter;
    }

    internal static FakeFarmer? Get_TauntFakeFarmer(this Monster monster)
    {
        return Values.TryGetValue(monster, out var value) ? value.FakeFarmer : null;
    }

    internal static void Set_Taunter(this Monster monster, Character? taunter)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.Taunter = taunter;
        holder.FakeFarmer = taunter is null
            ? null
            : taunter as FakeFarmer ?? new FakeFarmer { UniqueMultiplayerID = monster.GetHashCode(), currentLocation = monster.currentLocation };
    }

    internal class Holder
    {
        public Character? Taunter { get; internal set; }

        public FakeFarmer? FakeFarmer { get; internal set; }
    }
}
