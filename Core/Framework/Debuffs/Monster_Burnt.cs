namespace DaLion.Core.Framework.Debuffs;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Monster_Burnt
{
    internal static ConditionalWeakTable<Monster, Holder> Values { get; } = [];

    internal static void Set_Burnt(this Monster monster, int timer, Farmer? burner)
    {
        var holder = Values.GetOrCreateValue(monster);
        holder.BurnTimer.Value = timer;
        holder.Burner = burner;
    }

    internal static NetDouble Get_BurnTimer(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).BurnTimer;
    }

    // Net types are readonly
    internal static void Set_BurnTimer(this Monster monster, NetInt value)
    {
    }

    internal static Farmer? Get_Burner(this Monster monster)
    {
        return Values.GetOrCreateValue(monster).Burner;
    }

    internal static void Set_Burner(this Monster monster, Farmer? burner)
    {
        Values.GetOrCreateValue(monster).Burner = burner;
    }

    // Avoid redundant hashing
    internal static Holder Get_BurnHolder(this Monster monster)
    {
        return Values.GetOrCreateValue(monster);
    }

    internal class Holder
    {
        public NetDouble BurnTimer { get; } = new(-1);

        public Farmer? Burner { get; internal set; }

        public void Deconstruct(out NetDouble timer, out Farmer? burner)
        {
            timer = this.BurnTimer;
            burner = this.Burner;
        }
    }
}
