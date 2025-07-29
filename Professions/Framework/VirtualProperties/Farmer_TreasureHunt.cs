namespace DaLion.Professions.Framework.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Netcode;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_TreasureHunt
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = [];

    internal static Holder Get_TreasureHunt(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer);
    }

    // Net types are readonly
    internal static void Set_TreasureHunt(this Farmer farmer, Holder value)
    {
    }

    internal class Holder
    {
        public NetBool IsHuntingTreasure => new(false);

        public string LocationNameOrUniqueName { get; internal set; } = string.Empty;

        public Vector2 TreasureTile { get; internal set; } = Vector2.Zero;
    }
}
