namespace DaLion.Ligo.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Ligo.Modules.Professions.TreasureHunts;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_TreasureHunt
{
    internal static ConditionalWeakTable<Farmer, ProspectorHunt> ProspectorHunts { get; } = new();

    internal static ConditionalWeakTable<Farmer, ScavengerHunt> ScavengerHunts { get; } = new();

    internal static ConditionalWeakTable<Farmer, NetBool> Values { get; } = new();

    internal static ProspectorHunt Get_ProspectorHunt(this Farmer farmer)
    {
        return ProspectorHunts.GetValue(farmer, _ => new ProspectorHunt());
    }

    internal static bool Get_HasProspectorHunt(this Farmer farmer)
    {
        return ProspectorHunts.TryGetValue(farmer, out _);
    }

    internal static ScavengerHunt Get_ScavengerHunt(this Farmer farmer)
    {
        return ScavengerHunts.GetValue(farmer, _ => new ScavengerHunt());
    }

    internal static bool Get_HasScavengerHunt(this Farmer farmer)
    {
        return ScavengerHunts.TryGetValue(farmer, out _);
    }

    internal static NetBool Get_IsHuntingTreasure(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer);
    }

    // Net types are readonly
    internal static void Set_IsHuntingTreasure(this Farmer farmer, NetBool value)
    {
    }
}
