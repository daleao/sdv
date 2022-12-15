namespace DaLion.Overhaul.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_KnockedBack
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static bool Get_KnockedBack(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).KnockedBack;
    }

    internal static void Set_KnockedBack(this Monster monster, bool value)
    {
        Values.GetOrCreateValue(monster).KnockedBack = value;
    }

    internal class Holder
    {
        public bool KnockedBack { get; internal set; }
    }
}
