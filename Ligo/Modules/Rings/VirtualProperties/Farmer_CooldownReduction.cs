namespace DaLion.Ligo.Modules.Rings.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Extensions.Stardew;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_CooldownReduction
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static float Get_CooldownReduction(this Farmer farmer)
    {
        return Values.GetValue(farmer, Create).CooldownReduction;
    }

    private static Holder Create(Farmer farmer)
    {
        return new Holder { CooldownReduction = 1f - (farmer.Read<float>(DataFields.CooldownReduction) * 0.1f) };
    }

    internal class Holder
    {
        public float CooldownReduction { get; internal set; }
    }
}
