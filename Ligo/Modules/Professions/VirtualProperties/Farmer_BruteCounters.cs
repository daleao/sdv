namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_BruteCounters
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static int Get_BruteKillCounter(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).KillCounter;
    }

    internal static void Set_BruteKillCounter(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).KillCounter = value;
    }

    internal static void Increment_BruteKillCounter(this Farmer farmer, int amount = 1)
    {
        Values.GetOrCreateValue(farmer).KillCounter += amount;
    }

    internal static int Get_BruteRageCounter(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).RageCounter;
    }

    internal static void Set_BruteRageCounter(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).RageCounter = value;
    }

    internal static void Increment_BruteRageCounter(this Farmer farmer, int amount = 1)
    {
        Values.GetOrCreateValue(farmer).RageCounter =
            Math.Min(Values.GetOrCreateValue(farmer).RageCounter + amount, 100);
    }

    internal static void Decrement_BruteRageCounter(this Farmer farmer, int amount = 1)
    {
        Values.GetOrCreateValue(farmer).RageCounter =
            Math.Max(Values.GetOrCreateValue(farmer).RageCounter - amount, 0);
    }

    internal class Holder
    {
        private int _killCounter;
        private int _rageCounter;

        public int KillCounter
        {
            get => this._killCounter;
            internal set
            {
                this._killCounter = value <= 0 ? 0 : value;
            }
        }

        public int RageCounter
        {
            get => this._rageCounter;
            internal set
            {
                this._rageCounter = value switch
                {
                    >= 100 => 100,
                    <= 0 => 0,
                    _ => value,
                };
            }
        }
    }
}
