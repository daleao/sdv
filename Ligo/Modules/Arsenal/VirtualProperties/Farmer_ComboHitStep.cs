namespace DaLion.Ligo.Modules.Arsenal.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using DaLion.Ligo.Modules.Arsenal.Events;
using StardewValley;

#endregion using directives

// ReSharper disable once InconsistentNaming
internal static class Farmer_ComboHitStep
{
    internal static ConditionalWeakTable<Farmer, Holder> Values { get; } = new();

    internal static ComboHitStep Get_CurrentHitStep(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).HitStep;
    }

    internal static void Set_CurrentHitStep(this Farmer farmer, ComboHitStep value)
    {
        Values.GetOrCreateValue(farmer).HitStep = value;
    }

    internal static void Increment_CurrentHitStep(this Farmer farmer)
    {
        Values.GetOrCreateValue(farmer).HitStep++;
    }

    internal static int Get_ComboCooldown(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).Cooldown;
    }

    internal static void Set_ComboCooldown(this Farmer farmer, int value)
    {
        Values.GetOrCreateValue(farmer).Cooldown = value;
    }

    internal static void Decrement_ComboCooldown(this Farmer farmer, int amount = 1)
    {
        Values.GetOrCreateValue(farmer).Cooldown -= amount;
    }

    internal static bool Get_IsAnimating(this Farmer farmer)
    {
        return Values.GetOrCreateValue(farmer).IsAnimating;
    }

    internal static void Set_IsAnimating(this Farmer farmer, bool value)
    {
        if (value)
        {
            ModEntry.Events.Disable<ComboResetUpdateTickedEvent>();
        }
        else
        {
            ModEntry.Events.Enable<ComboResetUpdateTickedEvent>();
        }

        Values.GetOrCreateValue(farmer).IsAnimating = value;
    }

    internal class Holder
    {
        private ComboHitStep _hitStep;

        public ComboHitStep HitStep
        {
            get => this._hitStep;
            internal set
            {
                Log.D($"{value}");
                this._hitStep = value;
            }
        }

        public int Cooldown { get; internal set; }

        public bool IsAnimating { get; internal set; }
    }
}
