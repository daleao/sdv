namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Stolen
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static int Get_Stolen(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Stolen;
    }

    internal static void IncrementStolen(this Monster monster)
    {
        Values.GetOrCreateValue(monster).Stolen++;
    }

    internal class Holder
    {
        public int Stolen { get; internal set; }
    }
}
