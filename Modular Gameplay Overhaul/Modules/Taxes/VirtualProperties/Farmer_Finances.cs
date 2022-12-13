namespace DaLion.Overhaul.Modules.Taxes.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_Finances
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static int Get_LatestDue(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).LatestAmountDue;
    }

    internal static void Set_LatestDue(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).LatestAmountDue = value;
    }

    internal static int Get_LatestCharge(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).LatestAmountCharged;
    }

    internal static void Set_LatestCharge(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).LatestAmountCharged = value;
    }

    internal class Holder
    {
        public int LatestAmountDue { get; internal set; }

        public int LatestAmountCharged { get; internal set; }
    }
}
