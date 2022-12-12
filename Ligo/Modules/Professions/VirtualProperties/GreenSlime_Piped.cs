namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;
using Ultimates;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Piped
{
    internal static ConditionalWeakTable<GreenSlime, PipedSlime> Values { get; } = new();

    internal static PipedSlime? Get_Piped(this GreenSlime slime)
    {
        return Values.TryGetValue(slime, out var piped) ? piped : null;
    }

    internal static void Set_Piped(this GreenSlime slime, Farmer piper)
    {
        Values.AddOrUpdate(slime, new PipedSlime(slime, piper));
    }
}
