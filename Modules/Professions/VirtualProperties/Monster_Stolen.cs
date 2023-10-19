namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Stolen
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static NetBool Get_Stolen(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Stolen;
    }

    // Net types are readonly
    internal static void Set_Stolen(this Monster monster, bool value)
    {

    }

    internal class Holder
    {
        public NetBool Stolen { get; } = new(false);
    }
}
