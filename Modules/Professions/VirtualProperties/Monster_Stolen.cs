namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Stolen
{
    internal static ConditionalWeakTable<Monster, NetBool> Values { get; } = new();

    internal static NetBool Get_Stolen(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    // Net types are readonly
    internal static void Set_Stolen(this Monster monster, NetBool value)
    {
    }
}
