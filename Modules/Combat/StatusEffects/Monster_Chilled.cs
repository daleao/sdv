namespace DaLion.Overhaul.Modules.Combat.StatusEffects;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Chilled
{
    internal static ConditionalWeakTable<Monster, NetBool> Values { get; } = new();

    internal static NetBool Get_Chilled(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    // Net types are readonly
    internal static void Set_Chilled(this Monster monster, NetBool value)
    {
    }
}
