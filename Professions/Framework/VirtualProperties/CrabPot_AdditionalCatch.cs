namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class CrabPot_AdditionalCatch
{
    internal static ConditionalWeakTable<CrabPot, Holder> Values { get; } = [];

    internal static int Get_CatchAttempts(this CrabPot crabPot)
    {
        return Values.GetOrCreateValue(crabPot).Attempts;
    }

    internal static void IncrementCatchAttempts(this CrabPot crabPot)
    {
        Values.GetOrCreateValue(crabPot).Attempts++;
    }

    internal static void ResetCatchAttempts(this CrabPot crabPot)
    {
        Values.GetOrCreateValue(crabPot).Attempts = 0;
    }

    internal static bool IsBlockedFromAdditionalCatches(this CrabPot crabPot)
    {
        return Values.GetOrCreateValue(crabPot).Blocked;
    }

    internal static void BlockAdditionalCatches(this CrabPot crabPot)
    {
        Values.GetOrCreateValue(crabPot).Blocked = true;
    }

    internal static void UnblockAdditionalCatches(this CrabPot crabPot)
    {
        Values.GetOrCreateValue(crabPot).Blocked = false;
    }

    internal class Holder
    {
        public int Attempts { get; internal set; }

        public bool Blocked { get; internal set; }
    }
}
