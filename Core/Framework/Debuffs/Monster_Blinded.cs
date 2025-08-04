namespace DaLion.Core.Framework.Debuffs;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Blinded
{
    internal static ConditionalWeakTable<Monster, NetDouble> Values { get; } = [];

    internal static NetDouble Get_BlindTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    // Net types are readonly
    internal static void Set_BlindTimer(this Monster monster, NetDouble value)
    {
    }
}
