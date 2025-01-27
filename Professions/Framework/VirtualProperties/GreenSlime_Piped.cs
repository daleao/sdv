namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Piped
{
    internal static ConditionalWeakTable<GreenSlime, PipedSlime> Values { get; } = [];

    internal static HashSet<GreenSlime> PipedSlimes { get; } = [];

    internal static PipedSlime? Get_Piped(this GreenSlime slime)
    {
        return Values.TryGetValue(slime, out var piped) ? piped : null;
    }

    internal static void Set_Piped(this GreenSlime slime, Farmer? piper, bool summoned = true)
    {
        if (piper is not null)
        {
            var piped = new PipedSlime(slime, piper, summoned);
            Values.AddOrUpdate(slime, piped);
            PipedSlimes.Add(slime);
        }
        else
        {
            if (!Values.TryGetValue(slime, out var piped))
            {
                return;
            }

            piped.Reset();
            piped.Dispose();
            PipedSlimes.Remove(slime);
            Values.Remove(slime);
        }
    }
}
