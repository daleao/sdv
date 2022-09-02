namespace DaLion.Stardew.Arsenal.Framework.VirtualProperties;

#region using directives

using StardewValley.Monsters;
using System.Runtime.CompilerServices;

#endregion using directives

public static class Monster_Overkill
{
    internal class Holder
    {
        public int overkill;
    }

    internal static ConditionalWeakTable<Monster, Holder> Values = new();

    public static int get_Overkill(this Monster monster) => Values.GetOrCreateValue(monster).overkill;

    public static void set_Overkill(this Monster monster, int newVal)
    {
        Values.GetOrCreateValue(monster).overkill = newVal;
    }
}