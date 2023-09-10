namespace DaLion.Overhaul.Modules.Combat.VirtualProperties;

#region using directives

using System.Linq;
using System.Runtime.CompilerServices;
using DaLion.Overhaul.Modules.Combat.Resonance;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_ResonantResilience
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static float Get_ResonantResilience(this Farmer farmer)
    {
        return Values.GetValue(farmer, Create).ResonantResilience;
    }

    internal static void IncrementResonantResilience(this Farmer farmer, float amount)
    {
        Values.GetValue(farmer, Create).ResonantResilience += amount;
    }

    private static Holder Create(Farmer farmer)
    {
        return new Holder
        {
            ResonantResilience = farmer
                .Get_ResonatingChords()
                .Aggregate(
                    0f,
                    (sum, chord) => sum + (chord.ResonanceByGemstone.TryGetValue(Gemstone.Topaz, out var resonance)
                        ? (float)resonance
                        : 0f)),
        };
    }

    internal class Holder
    {
        public float ResonantResilience { get; internal set; }
    }
}
