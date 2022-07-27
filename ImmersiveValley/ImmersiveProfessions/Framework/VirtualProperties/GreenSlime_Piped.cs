namespace DaLion.Stardew.Professions.Framework.VirtualProperties;

#region using directives

using Netcode;
using StardewValley;
using StardewValley.Monsters;
using System.Runtime.CompilerServices;

#endregion using directives

public static class GreenSlime_Piped
{
    internal class Holder
    {
        public readonly NetInt pipeTimer = new(-1);
        public Farmer? piper;
        public Farmer? fakeFarmer;
        public float originalScale;
        public int originalHealth;
        public int originalRange;
        public bool inflated;
    }

    internal static ConditionalWeakTable<GreenSlime, Holder> Values = new();

    public static NetInt get_PipeTimer(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.pipeTimer;
    }

    public static void set_PipeTimer(this GreenSlime slime, NetInt newVal)
    {
        // Net types should not have a setter as they are readonly
    }

    public static Farmer? get_Piper(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.piper;
    }

    public static void set_Piper(this GreenSlime slime, Farmer? piper)
    {
        var holder = Values.GetOrCreateValue(slime);
        holder.piper = piper;
        holder.pipeTimer.Value = (int)(30000 / ModEntry.Config.SpecialDrainFactor);
        holder.originalHealth = slime.MaxHealth;
        holder.originalScale = slime.Scale;
        holder.originalRange = slime.moveTowardPlayerThreshold.Value;
        holder.fakeFarmer = new() { UniqueMultiplayerID = slime.GetHashCode(), currentLocation = slime.currentLocation };
    }

    public static Farmer? get_FakeFarmer(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.fakeFarmer;
    }

    public static float get_OriginalScale(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.originalScale;
    }

    public static int get_OriginalHealth(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.originalHealth;
    }

    public static int get_OriginalRange(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.originalRange;
    }

    public static bool get_Inflated(this GreenSlime slime)
    {
        var holder = Values.GetOrCreateValue(slime);
        return holder.inflated;
    }

    public static void set_Inflated(this GreenSlime slime, bool newVal)
    {
        var holder = Values.GetOrCreateValue(slime);
        holder.inflated = newVal;
    }
}