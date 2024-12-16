namespace DaLion.Combat.Framework.VirtualProperties;

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

    internal static void Set_GotCrit(this Monster monster, Farmer? byWhom)
    {
        Values.GetOrCreateValue(monster).ByWhom = byWhom;
    }

    internal class Holder
    {
        public bool GotCrit => this.ByWhom is not null;

        public Farmer? ByWhom { get; internal set; }
    }
}
