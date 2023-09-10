namespace DaLion.Overhaul.Modules.Professions.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

internal static class ArrowProjectile_Properties
{
    internal static ConditionalWeakTable<BasicProjectile, Holder> Values { get; } = new();

    internal static void Create(
        BasicProjectile projectile,
        Slingshot source,
        float overcharge,
        bool canPierce)
    {
        Values.AddOrUpdate(
            projectile,
            new Holder(
                source,
                overcharge,
                canPierce));
    }

    internal static Slingshot? Get_Source(this BasicProjectile projectile)
    {
        return Values.TryGetValue(projectile, out var holder) ? holder.Source : null;
    }

    internal static float Get_Overcharge(this BasicProjectile projectile)
    {
        return Values.TryGetValue(projectile, out var holder) ? holder.Overcharge : 1f;
    }

    internal static void ModiftyOvercharge(this BasicProjectile projectile, float modifier)
    {
        if (Values.TryGetValue(projectile, out var holder))
        {
            holder.Overcharge *= modifier;
        }
    }

    internal static bool Get_CanPierce(this BasicProjectile projectile)
    {
        return Values.TryGetValue(projectile, out var holder) && holder.CanPierce;
    }

    internal static bool Get_DidPierce(this BasicProjectile projectile)
    {
        return Values.TryGetValue(projectile, out var holder) && holder.DidPierce;
    }

    internal static void Set_DidPierce(this BasicProjectile projectile, bool newValue)
    {
        if (Values.TryGetValue(projectile, out var holder))
        {
            holder.DidPierce = newValue;
        }
    }

    internal class Holder
    {
        internal Holder(
            Slingshot source,
            float overcharge,
            bool canPierce)
        {
            this.Source = source;
            this.Overcharge = overcharge;
            this.CanPierce = canPierce;
        }

        public Slingshot Source { get; internal set; }

        public float Overcharge { get; internal set; }

        public bool CanPierce { get; internal set; }

        public bool DidPierce { get; internal set; }
    }
}
