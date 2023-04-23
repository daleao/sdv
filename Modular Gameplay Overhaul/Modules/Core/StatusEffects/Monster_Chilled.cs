namespace DaLion.Overhaul.Modules.Core.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Chilled
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetBool Get_Chilled(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Chilled;
    }

    // Net types are readonly
    internal static void Set_Chilled(this Monster monster, NetBool value)
    {
    }

    internal class Holder
    {
        public NetBool Chilled { get; } = new(false);
    }
}
