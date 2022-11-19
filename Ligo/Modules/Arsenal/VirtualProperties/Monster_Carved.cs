namespace DaLion.Ligo.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Carved
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static int Get_Carved(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Carved;
    }

    internal static void Set_Carved(this Monster monster, int newVal)
    {
        Values.GetOrCreateValue(monster).Carved = newVal;
    }

    internal static void Increment_Carved(this Monster monster)
    {
        ++Values.GetOrCreateValue(monster).Carved;
    }

    internal class Holder
    {
        public int Carved { get; internal set; }
    }
}
