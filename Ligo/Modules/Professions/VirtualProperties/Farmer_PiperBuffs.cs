namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

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

    internal class Holder
    {
        public int[] AppliedPiperBuffs { get; } = new int[12];
    }
}
