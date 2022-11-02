namespace DaLion.Redux.Framework.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Feared
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetInt Get_FearTimer(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.FearTimer;
    }

    // Net types are readonly
    internal static void Set_FearTimer(this Monster monster, NetInt newVal)
    {
    }

    internal static NetInt Get_FearIntensity(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.FearIntensity;
    }

    // Net types are readonly
    internal static void Set_FearIntensity(this Monster monster, NetInt newVal)
    {
    }

    internal static Farmer Get_Fearer(this Monster monster)
    {
        var holder = Values.GetOrCreateValue(monster);
        return holder.Fearer;
    }

    internal static void Set_Fearer(this Monster monster, Farmer fearer)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.Fearer = fearer;
    }

    internal class Holder
    {
        public NetInt FearIntensity { get;  } = new(-1);

        public NetInt FearTimer { get; } = new(-1);

        public Farmer Fearer { get; internal set; } = null!;
    }
}
