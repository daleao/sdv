namespace DaLion.Ligo.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Overkill
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static int Get_Overkill(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Overkill;
    }

    internal static void Set_Overkill(this Monster monster, int newVal)
    {
        Values.GetOrCreateValue(monster).Overkill = newVal;
    }

    internal class Holder
    {
        public int Overkill { get; internal set; }
    }
}
