namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Feared
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetInt Get_FearTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).FearTimer;
    }

    // Net types are readonly
    internal static void Set_FearTimer(this Monster monster, NetInt value)
    {
    }

    internal class Holder
    {
        public NetInt FearTimer { get; } = new(-1);
    }
}
