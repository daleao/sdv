namespace DaLion.Stardew.Arsenal.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_GotCrit
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static bool Get_GotCrit(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).GotCrit;
    }

    internal static void Set_GotCrit(this Monster monster, bool newVal)
    {
        Values.GetOrCreateValue(monster).GotCrit = newVal;
    }

    internal class Holder
    {
        public bool GotCrit { get; internal set; }
    }
}
