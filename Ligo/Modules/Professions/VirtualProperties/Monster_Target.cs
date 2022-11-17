namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Target
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static Farmer Get_Target(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Target ?? Game1.player;
    }

    internal static void Set_Target(this Monster monster, Farmer? target)
    {
        Values.GetOrCreateValue(monster).Target = target;
    }

    internal class Holder
    {
        public Farmer? Target { get; internal set; }
    }
}
