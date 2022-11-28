namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_SpelunkerLadderStreak
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static int Get_SpelunkerLadderStreak(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).LadderStreak;
    }

    internal static void Set_SpelunkerLadderStreak(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).LadderStreak = value;
    }

    internal static void Increment_SpelunkerLadderStreak(this Farmer farmer, int amount = 1)
    {
        Values.GetOrCreateValue(farmer).LadderStreak += amount;
    }

    internal class Holder
    {
        public int LadderStreak { get; internal set; }
    }
}
