namespace DaLion.Stardew.Slingshots.Framework.VirtualProperties;

#region using directives

using StardewValley.Tools;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Slingshot_OnSpecial
{
    internal class Holder
    {
        public bool isOnSpecial;
    }

    internal static ConditionalWeakTable<Slingshot, Holder> Values = new();

    public static bool get_IsOnSpecial(this Slingshot slingshot) => Values.GetOrCreateValue(slingshot).isOnSpecial;

    public static void set_IsOnSpecial(this Slingshot slingshot, bool newVal)
    {
        Values.GetOrCreateValue(slingshot).isOnSpecial = newVal;
    }
}