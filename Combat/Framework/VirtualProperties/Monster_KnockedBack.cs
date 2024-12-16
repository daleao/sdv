namespace DaLion.Combat.Framework.VirtualProperties;

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

    internal static Farmer? Get_KnockBacker(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).ByWhom;
    }

    internal static void Set_KnockedBack(this Monster monster, Farmer? byWhom)
    {
        Values.GetOrCreateValue(monster).ByWhom = byWhom;
    }

    internal class Holder
    {
        public bool KnockedBack => this.ByWhom is not null;

        public Farmer? ByWhom { get; internal set; }
    }
}
