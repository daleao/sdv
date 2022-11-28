namespace DaLion.Ligo.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_WasKnockedBack
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static bool Get_WasKnockedBack(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).WasKnockedBack;
    }

    internal static void Set_WasKnockedBack(this Monster monster, bool value)
    {
        Values.GetOrCreateValue(monster).WasKnockedBack = value;
    }

    internal class Holder
    {
        public bool WasKnockedBack { get; internal set; }
    }
}
