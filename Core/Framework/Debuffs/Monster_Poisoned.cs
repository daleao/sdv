namespace DaLion.Core.Framework.Debuffs;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Shared.Extensions.Stardew;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Poisoned
{
    internal const int INSTA_KILL_HEALTH_THRESHOLD = 2000;

    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = [];

    internal static bool Increment_Poisoned(this Monster monster, int timer, int stacks, Farmer? poisoner, int maxStacks = int.MaxValue)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.PoisonTimer.Value = timer;
        holder.PoisonStacks.Value += stacks;
        if (holder.PoisonStacks.Value >= maxStacks && monster.Health < INSTA_KILL_HEALTH_THRESHOLD)
        {
            monster.Die(poisoner);
            return false;
        }

        holder.Poisoner = poisoner;
        return true;
    }

    internal static void Set_Poisoned(this Monster monster, int timer, int stacks, Farmer? poisoner)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.PoisonTimer.Value = timer;
        holder.PoisonStacks.Value = stacks;
        holder.Poisoner = poisoner;
    }

    internal static NetDouble Get_PoisonTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).PoisonTimer;
    }

    // Net types are readonly
    internal static void Set_PoisonTimer(this Monster monster, NetDouble value)
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

    // Avoid redundant hashing
    internal static Holder Get_PoisonHolder(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    internal class Holder
    {
        public NetDouble PoisonTimer { get; } = new(-1);

        public NetInt PoisonStacks { get; internal set; } = new(0);

        public Farmer? Poisoner { get; internal set; }

        public void Deconstruct(out NetDouble timer, out NetInt stacks, out Farmer? poisoner)
        {
            timer = this.PoisonTimer;
            stacks = this.PoisonStacks;
            poisoner = this.Poisoner;
        }
    }
}
