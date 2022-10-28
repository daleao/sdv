namespace DaLion.Redux.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_KnockbackCooldown
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static int Get_KnockbackCooldown(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).KnockbackCooldown;
    }

    internal static void Set_KnockbackCooldown(this Monster monster, int newVal)
    {
        Values.GetOrCreateValue(monster).KnockbackCooldown = newVal;
    }

    internal class Holder
    {
        public int KnockbackCooldown { get; internal set; }
    }
}
