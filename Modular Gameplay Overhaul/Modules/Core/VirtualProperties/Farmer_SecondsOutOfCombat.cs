namespace DaLion.Overhaul.Modules.Core.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_SecondsOutOfCombat
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static int Get_SecondsOutOfCombat(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).SecondsOutOfCombat;
    }

    internal static void Set_SecondsOutOfCombat(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).SecondsOutOfCombat = value;
    }

    internal static void Increment_SecondsOutOfCombat(this Farmer farmer, int amount = 1)
    {
        Values.GetOrCreateValue(farmer).SecondsOutOfCombat += amount;
    }

    internal class Holder
    {
        public int SecondsOutOfCombat { get; internal set; }
    }
}
