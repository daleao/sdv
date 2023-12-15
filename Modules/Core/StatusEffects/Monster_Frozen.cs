namespace DaLion.Overhaul.Modules.Core.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Frozen
{
    internal static ConditionalWeakTable<Monster, NetBool> Values { get; } = new();

    internal static NetBool Get_Frozen(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    // Net types are readonly
    internal static void Set_Frozen(this Monster monster, NetBool value)
    {
    }
}
