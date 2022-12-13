namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_DemolitionistExcitedness
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static int Get_DemolitionistExcitedness(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).Excitedness;
    }

    internal static void Set_DemolitionistExcitedness(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).Excitedness = value;
    }

    internal static void Increment_DemolitionistExcitedness(this Farmer farmer, int amount = 1)
    {
        Values.GetOrCreateValue(farmer).Excitedness += amount;
    }

    internal static void Decrement_DemolitionistExcitedness(this Farmer farmer, int amount = 1)
    {
        Values.GetOrCreateValue(farmer).Excitedness -= amount;
    }

    internal class Holder
    {
        private int _excitedness;

        public int Excitedness
        {
            get => this._excitedness;
            internal set
            {
                this._excitedness = value <= 0 ? 0 : value;
            }
        }
    }
}
