namespace DaLion.Ligo.Modules.Rings.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_RingBuffs
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static int Get_SavageExcitedness(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).SavageExcitedness;
    }

    internal static void Set_SavageExcitedness(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).SavageExcitedness = value;
    }

    internal static int Get_WarriorKillCount(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).WarriorKillCount;
    }

    internal static void Set_WarriorKillCount(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).WarriorKillCount = value;
    }

    internal static void Increment_WarriorKillCount(this Farmer farmer)
    {
        Values.GetOrCreateValue(farmer).WarriorKillCount++;
    }

    internal class Holder
    {
        public int SavageExcitedness { get; internal set; }

        public int WarriorKillCount { get; internal set; }
    }
}
