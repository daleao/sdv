namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Slowed
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetInt Get_SlowTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).SlowTimer;
    }

    // Net types are readonly
    internal static void Set_SlowTmer(this Monster monster, NetInt value)
    {
    }

    internal static NetInt Get_SlowIntensity(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).SlowIntensity;
    }

    // Net types are readonly
    internal static void Set_SlowIntensity(this Monster monster, NetInt value)
    {
    }

    internal static Farmer Get_Slower(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Slower;
    }

    internal static void Set_Slower(this Monster monster, Farmer slower)
    {
        Values.GetOrCreateValue(monster).Slower = slower;
    }

    internal class Holder
    {
        public NetInt SlowIntensity { get; } = new(-1);

        public NetInt SlowTimer { get; } = new(-1);

        public Farmer Slower { get; internal set; } = null!;
    }
}
