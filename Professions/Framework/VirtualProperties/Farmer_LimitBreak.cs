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

    public static NetString Get_LimitBreakId(this Farmer farmer)
    {
        return Values.GetValue(farmer, Create).Id;
    }

    // Net types are readonly
    public static void Set_LimitBreakId(this Farmer farmer, NetString value)
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
        var id = Data.Read(farmer, DataKeys.LimitBreakId);
        return new Holder
        {
            Id = { Value = id },
            LimitBreak = string.IsNullOrEmpty(id) ? null : LimitBreak.FromName(id),
        };
    }

    public class Holder
    {
        public NetString Id { get; } = new(string.Empty);

        public NetBool IsActive { get; } = new(false);

        public LimitBreak? LimitBreak { get; internal set; } = null;
    }
}
