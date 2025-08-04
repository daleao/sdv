namespace DaLion.Core.Framework.Debuffs;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Slowed
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = [];

    internal static void SetOrIncrement_Slowed(this Monster monster, int timer, float intensity)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.SlowTimer.Value = timer;
        holder.SlowIntensity.Value += intensity;
    }

    internal static void Set_Slowed(this Monster monster, int timer, float intensity)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.SlowTimer.Value = timer;
        holder.SlowIntensity.Value = intensity;
    }

    internal static NetDouble Get_SlowTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).SlowTimer;
    }

    // Net types are readonly
    internal static void Set_SlowTimer(this Monster monster, NetDouble value)
    {
    }

    internal static NetFloat Get_SlowIntensity(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).SlowIntensity;
    }

    // Net types are readonly
    internal static void Set_SlowIntensity(this Monster monster, NetInt value)
    {
    }

    // Avoid redundant hashing
    internal static Holder Get_SlowHolder(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    internal class Holder
    {
        public NetDouble SlowTimer { get; } = new(-1);

        public NetFloat SlowIntensity { get; } = new(0);

        public void Deconstruct(out NetDouble timer, out NetFloat intensity)
        {
            timer = this.SlowTimer;
            intensity = this.SlowIntensity;
        }
    }
}
