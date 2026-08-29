namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using StardewValley.Monsters;
using StardewValley.Objects;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Piped
{
    internal static ConditionalWeakTable<GreenSlime, PipedSlime> Values { get; } = [];

    internal static HashSet<GreenSlime> PipedSlimes { get; } = [];

    internal static bool IsPiped(this GreenSlime slime)
    {
        return slime.Get_Piped() is not null;
    }

    internal static PipedSlime? Get_Piped(this GreenSlime slime)
    {
        return Values.TryGetValue(slime, out var piped) ? piped : null;
    }

    [return: NotNullIfNotNull(nameof(piper))]
    internal static PipedSlime? Set_Piped(this GreenSlime slime, Farmer piper, PipedSlime.PipingSource source)
    {
        var piped = new PipedSlime(slime, piper, source);
        Values.AddOrUpdate(slime, piped);
        PipedSlimes.Add(slime);
        slime.MaxHealth *= 5;
        slime.Health = slime.MaxHealth;
        return piped;
    }

    internal static PipedSlime Set_Piped(this GreenSlime slime, Farmer piper, Hat hat)
    {
        var piped = new PipedSlime(slime, piper, hat);
        Values.AddOrUpdate(slime, piped);
        PipedSlimes.Add(slime);
        return piped;
    }

    internal static bool Unpipe(this GreenSlime slime)
    {
        if (!Values.TryGetValue(slime, out var piped))
        {
            return false;
        }

        piped.Reset();
        piped.Dispose();
        PipedSlimes.Remove(slime);
        Values.Remove(slime);
        slime.MaxHealth /= 5;
        slime.Health = slime.MaxHealth;
        return true;
    }
}
