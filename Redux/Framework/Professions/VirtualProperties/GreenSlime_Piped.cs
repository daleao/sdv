namespace DaLion.Redux.Framework.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using Netcode;
using StardewValley.Monsters;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class GreenSlime_Piped
{
    internal static ConditionalWeakTable<GreenSlime, Holder> Values { get; } = new();

    internal static NetInt Get_PipeTimer(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.PipeTimer;
    }

    // Net types are readonly
    internal static void Set_PipeTimer(this GreenSlime slime, NetInt newVal)
    {
    }

    internal static Farmer? Get_Piper(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.Piper;
    }

    internal static void Set_Piper(this GreenSlime slime, Farmer? piper)
    {
        var holder = Values.GetOrCreateValue(slime);
        holder.Piper = piper;
        holder.PipeTimer.Value = (int)(30000 / ModEntry.Config.Professions.SpecialDrainFactor);
        holder.OriginalHealth = slime.MaxHealth;
        holder.OriginalScale = slime.Scale;
        holder.OriginalRange = slime.moveTowardPlayerThreshold.Value;
        holder.FakeFarmer = new Farmer
        {
            UniqueMultiplayerID = slime.GetHashCode(), currentLocation = slime.currentLocation,
        };
    }

    internal static Farmer? Get_FakeFarmer(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.FakeFarmer;
    }

    internal static float Get_OriginalScale(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.OriginalScale;
    }

    internal static int Get_OriginalHealth(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.OriginalHealth;
    }

    internal static int Get_OriginalRange(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.OriginalRange;
    }

    internal static bool Get_Inflated(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.Inflated;
    }

    internal static void Set_Inflated(this GreenSlime slime, bool newVal)
    {
        var holder = Values.GetOrCreateValue(slime);
        holder.Inflated = newVal;
    }

    internal class Holder
    {
        public NetInt PipeTimer { get; } = new(-1);

        public Farmer? FakeFarmer { get; internal set; }

        public bool Inflated { get; internal set; }

        public int OriginalHealth { get; internal set; }

        public int OriginalRange { get; internal set; }

        public float OriginalScale { get; internal set; }

        public Farmer? Piper { get; internal set; }
    }
}
