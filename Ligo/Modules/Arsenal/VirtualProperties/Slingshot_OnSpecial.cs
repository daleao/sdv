namespace DaLion.Ligo.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Tools;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Slingshot_OnSpecial
{
    internal static ConditionalWeakTable<Slingshot, Holder> Values { get; } = new();

    internal static bool Get_IsOnSpecial(this Slingshot slingshot)
    {
        return Values.GetOrCreateValue(slingshot).IsOnSpecial;
    }

    internal static void Set_IsOnSpecial(this Slingshot slingshot, bool value)
    {
        Values.GetOrCreateValue(slingshot).IsOnSpecial = value;
    }

    internal static int Get_SpecialCooldown(this Slingshot slingshot)
    {
        return Values.GetOrCreateValue(slingshot).SpecialCooldown;
    }

    internal static void Set_SpecialCooldown(this Slingshot slingshot, int value)
    {
        Values.GetOrCreateValue(slingshot).SpecialCooldown = value;
    }

    internal static void Decrement_SpecialCooldown(this Slingshot slingshot, int amount = 1)
    {
        Values.GetOrCreateValue(slingshot).SpecialCooldown -= amount;
    }

    internal static void Halve_SpecialCooldown(this Slingshot slingshot)
    {
        Values.GetOrCreateValue(slingshot).SpecialCooldown /= 2;
    }

    internal class Holder
    {
        public bool IsOnSpecial { get; internal set; }

        public int SpecialCooldown { get; internal set; }
    }
}
