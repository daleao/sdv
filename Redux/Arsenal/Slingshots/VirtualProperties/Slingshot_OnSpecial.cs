namespace DaLion.Redux.Arsenal.Slingshots.VirtualProperties;

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

    internal static void Set_IsOnSpecial(this Slingshot slingshot, bool newVal)
    {
        Values.GetOrCreateValue(slingshot).IsOnSpecial = newVal;
    }

    internal class Holder
    {
        public bool IsOnSpecial { get; internal set; }
    }
}
