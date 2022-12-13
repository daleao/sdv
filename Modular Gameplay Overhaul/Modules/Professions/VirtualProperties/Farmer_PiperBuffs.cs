namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Linq;
using System.Runtime.CompilerServices;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_PiperBuffs
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static int[] Get_PiperBuffs(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).AppliedPiperBuffs;
    }

    internal static bool HasAnyPiperBuffs(this Farmer farmer)
    {
        return Values.TryGetValue(farmer, out var holder) && holder.AppliedPiperBuffs.Any(i => i > 0);
    }

    internal class Holder
    {
        public int[] AppliedPiperBuffs { get; } = new int[12];
    }
}
