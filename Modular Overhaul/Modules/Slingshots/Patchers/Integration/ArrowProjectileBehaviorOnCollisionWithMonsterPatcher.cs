namespace DaLion.Overhaul.Modules.Slingshots.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Slingshots.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "1.2.0")]
internal sealed class ArrowProjectileBehaviorOnCollisionWithMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ArrowProjectileBehaviorOnCollisionWithMonsterPatcher"/> class.</summary>
    internal ArrowProjectileBehaviorOnCollisionWithMonsterPatcher()
    {
        this.Target = "Archery.Framework.Objects.Projectiles.ArrowProjectile"
            .ToType()
            .RequireMethod("behaviorOnCollisionWithMonster");
    }

    #region harmony patches

    /// <summary>Apply stat modifiers to projectile.</summary>
    [HarmonyPrefix]
    private static void ArrowProjectileBehaviorOnCollisionWithMonsterPrefix(
        BasicProjectile __instance,
        ref int ____collectiveDamage,
        ref float ____criticalChance,
        ref float ____criticalDamageMultiplier,
        ref float ____knockback)
    {
        var source = __instance.Get_Source();
        if (source is null)
        {
            return;
        }

        ____collectiveDamage = (int)(____collectiveDamage * source.Get_RubyDamageModifier());
        ____criticalChance *= source.Get_AquamarineCritChanceModifier();
        ____criticalDamageMultiplier *= source.Get_JadeCritPowerModifier();
        ____knockback *= source.Get_AmethystKnockbackModifer();
    }

    #endregion harmony patches
}
