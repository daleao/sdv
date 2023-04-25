namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Poisoned
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetInt Get_PoisonTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).PoisonTimer;
    }

    // Net types are readonly
    internal static void Set_PoisonTimer(this Monster monster, NetInt value)
    {
    }

    internal static NetInt Get_PoisonStacks(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).PoisonStacks;
    }

    // Net types are readonly
    internal static void Set_PoisonStacks(this Monster monster, NetInt stacks)
    {
    }

    internal static Farmer? Get_Poisoner(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Poisoner;
    }

    internal static void Set_Poisoner(this Monster monster, Farmer? poisoner)
    {
        Values.GetOrCreateValue(monster).Poisoner = poisoner;
    }

    internal class Holder
    {
        public NetInt PoisonTimer { get; } = new(-1);

        public NetInt PoisonStacks { get; internal set; } = new(0);

        public Farmer? Poisoner { get; internal set; }
    }
}
