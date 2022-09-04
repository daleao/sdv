namespace DaLion.Stardew.Tweex.Framework.VirtualProperties;

#region using directives

using Netcode;
using StardewValley.TerrainFeatures;
using System.Runtime.CompilerServices;

#endregion using directives

public static class FruitTree_Fertilized
{
    internal class Holder
    {
        public NetBool fertilized = new(false);
    }

    internal static ConditionalWeakTable<FruitTree, Holder> Values = new();

    public static NetBool get_Fertilized(this FruitTree tree)
    {
        var holder = Values.GetOrCreateValue(tree);
        return holder.fertilized;
    }

    // Net types are readonly
    public static void set_Fertilzied(this FruitTree tree, NetBool newVal) { }
}