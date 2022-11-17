namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Stolen
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static bool Get_Stolen(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).WasStolen;
    }

    internal static void Set_Stolen(this Monster monster, bool newVal)
    {
        Values.GetOrCreateValue(monster).WasStolen = newVal;
    }

    internal class Holder
    {
        public bool WasStolen { get; internal set; }
    }
}
