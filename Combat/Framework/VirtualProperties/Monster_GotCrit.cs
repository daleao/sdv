namespace DaLion.Combat.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_GotCrit
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = new();

    internal static bool Get_GotCrit(this Monster monster)
    {
        return Values.TryGetValue(monster, out var gotCrit) && gotCrit.ByWhom is not null;
    }

    internal static float Get_GotCritMultiplier(this Monster monster)
    {
        return Values.TryGetValue(monster, out var gotCrit) ? gotCrit.Multiplier : 1f;
    }

    internal static void Set_GotCrit(this Monster monster, Farmer? byWhom, float multiplier)
    {
        var gotCrit = Values.GetOrCreateValue(monster);
        gotCrit.ByWhom = byWhom;
        gotCrit.Multiplier = multiplier;
    }

    internal class Holder
    {
        public bool GotCrit => this.ByWhom is not null;

        public Farmer? ByWhom { get; internal set; }

        public float Multiplier { get; internal set; }
    }
}
