namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Jumping
{
    internal static ConditionalWeakTable<GreenSlime, Holder> Values { get; } = new();

    internal static bool Get_Jumping(this GreenSlime slime)
    {
        return Values.GetOrCreateValue(slime).JumpTimer > 0;
    }

    internal static int Get_JumpTimer(this GreenSlime slime)
    {
        return Values.GetOrCreateValue(slime).JumpTimer;
    }

    internal static void Set_JumpTimer(this GreenSlime slime, int newVal)
    {
        Values.GetOrCreateValue(slime).JumpTimer = newVal;
    }

    internal class Holder
    {
        public int JumpTimer { get; internal set; }
    }
}
