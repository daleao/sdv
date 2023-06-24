namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class CrabPot_Updated
{
    internal static ConditionalWeakTable<CrabPot, Holder> Values { get; } = new();

    internal static int Get_Attempts(this CrabPot crabPot)
    {
        return Values.GetOrCreateValue(crabPot).Attempts;
    }

    internal static void IncrementAttempts(this CrabPot crabPot)
    {
        Values.GetOrCreateValue(crabPot).Attempts++;
    }

    internal static void ResetAttempts(this CrabPot crabPot)
    {
        Values.GetOrCreateValue(crabPot).Attempts = 0;
    }

    internal static int Get_Successes(this CrabPot crabPot)
    {
        return Values.GetOrCreateValue(crabPot).Successes;
    }

    internal static void IncrementSuccesses(this CrabPot crabPot)
    {
        Values.GetOrCreateValue(crabPot).Successes++;
    }

    internal static void ResetSuccesses(this CrabPot crabPot)
    {
        Values.GetOrCreateValue(crabPot).Successes = 0;
    }

    internal class Holder
    {
        public int Attempts { get; internal set; }

        public int Successes { get; internal set; }
    }
}
