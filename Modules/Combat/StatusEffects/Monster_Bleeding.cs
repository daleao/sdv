namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Bleeding
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static void SetOrIncrement_Bleeding(this Monster monster, int timer, int stacks, Farmer? bleeder)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.BleedTimer.Value = timer;
        holder.BleedStacks.Value = Math.Min(holder.BleedStacks.Value + stacks, 5);
        holder.Bleeder = bleeder;
    }

    internal static void Set_Bleeding(this Monster monster, int timer, int stacks, Farmer? bleeder)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.BleedTimer.Value = timer;
        holder.BleedStacks.Value = stacks;
        holder.Bleeder = bleeder;
    }

    internal static NetInt Get_BleedTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).BleedTimer;
    }

    // Net types are readonly
    internal static void Set_BleedTimer(this Monster monster, NetInt value)
    {
    }

    internal static NetInt Get_BleedStacks(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).BleedStacks;
    }

    // Net types are readonly
    internal static void Set_BleedStacks(this Monster monster, NetInt stacks)
    {
    }

    internal static Farmer? Get_Bleeder(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Bleeder;
    }

    internal static void Set_Bleeder(this Monster monster, Farmer? bleeder)
    {
        Values.GetOrCreateValue(monster).Bleeder = bleeder;
    }

    internal class Holder
    {
        public NetInt BleedTimer { get; } = new(-1);

        public NetInt BleedStacks { get; internal set; } = new(0);

        public Farmer? Bleeder { get; internal set; }
    }
}
