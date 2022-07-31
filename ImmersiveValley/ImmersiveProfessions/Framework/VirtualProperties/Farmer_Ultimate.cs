namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using Common.Extensions.Stardew;
using Netcode;
using System.Runtime.CompilerServices;
using Ultimates;

#endregion using directives

public static class Farmer_Ultimate
{
    internal class Holder
    {
        public Ultimate? ultimate;
        public readonly NetInt ultimateIndex = new(-1);
        public readonly NetBool isUltimateActive = new(false);
    }

    internal static ConditionalWeakTable<Farmer, Holder> Values = new();

    public static NetInt get_UltimateIndex(this Farmer farmer)
    {
        var holder = Values.GetOrCreateValue(farmer);
        return holder.ultimateIndex;
    }

    // Net types are readonly
    public static void set_UltimateIndex(this Farmer farmer, NetInt newVal) { }

    public static NetBool get_IsUltimateActive(this Farmer farmer)
    {
        var holder = Values.GetOrCreateValue(farmer);
        return holder.isUltimateActive;
    }

    // Net types are readonly
    public static void set_IsUltimateActive(this Farmer farmer, NetBool newVal) { }

    public static Ultimate? get_Ultimate(this Farmer farmer)
    {
        var holder = Values.GetOrCreateValue(farmer);
        return holder.ultimate;
    }

    public static void set_Ultimate(this Farmer farmer, Ultimate? newVal)
    {
        var holder = Values.GetOrCreateValue(farmer);
        holder.ultimate = newVal;

        var newIndex = newVal?.Index ?? UltimateIndex.None;
        holder.ultimateIndex.Value = (int)newIndex;
        farmer.Write("UltimateIndex", newIndex.ToString());
    }
}