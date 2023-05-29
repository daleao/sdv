namespace DaLion.Overhaul.Modules.Slingshots.VirtualProperties;

#region using directives

using System.Runtime.CompilerServices;
using StardewValley.Projectiles;
using StardewValley.Tools;

#endregion using directives

internal static class ArrowProjectile_Properties
{
    internal static ConditionalWeakTable<BasicProjectile, Slingshot> Values { get; } = new();

    internal static void Create(BasicProjectile projectile, Slingshot source)
    {
        Values.AddOrUpdate(projectile, source);
    }

    internal static Slingshot? Get_Source(this BasicProjectile projectile)
    {
        return Values.TryGetValue(projectile, out var source) ? source : null;
    }
}
