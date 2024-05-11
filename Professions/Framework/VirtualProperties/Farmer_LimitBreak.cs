namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Professions.Framework.Limits;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_LimitBreak
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = [];

    public static LimitBreak? Get_LimitBreak(this Farmer farmer)
    {
        return farmer.IsLocalPlayer
            ? State.LimitBreak
            : Values.GetOrCreateValue(farmer).LimitBreak;
    }

    public static void Set_LimitBreak(this Farmer farmer, LimitBreak? value)
    {
        if (farmer.IsLocalPlayer)
        {
            State.LimitBreak = value;
            return;
        }

        Values.GetOrCreateValue(farmer).LimitBreak = value;
    }

    public static NetInt Get_LimitBreakId(this Farmer farmer)
    {
        return Values.GetValue(farmer, Create).Id;
    }

    // Net types are readonly
    public static void Set_LimitBreakId(this Farmer farmer, NetInt value)
    {
    }

    public static NetBool Get_IsLimitBreaking(this Farmer farmer)
    {
        return Values.GetValue(farmer, Create).IsActive;
    }

    // Net types are readonly
    public static void Set_IsLimitBreaking(this Farmer farmer, NetBool value)
    {
    }

    private static Holder Create(Farmer farmer)
    {
        var id = Data.ReadAs(farmer, DataKeys.LimitBreakId, -1);
        return new Holder
        {
            Id = { Value = id },
            LimitBreak = id < 0 ? null : LimitBreak.FromId(id),
        };
    }

    public class Holder
    {
        public NetInt Id { get; } = new(-1);

        public NetBool IsActive { get; } = new(false);

        public LimitBreak? LimitBreak { get; internal set; } = null;
    }
}
