namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Integrations;
using DaLion.Overhaul.Modules.Professions.Ultimates;
using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using DaLion.Shared.Integrations.Archery;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Projectiles;

#endregion using directives

[UsedImplicitly]
[RequiresMod("PeacefulEnd.Archery", "Archery", "1.2.0")]
internal sealed class ArrowProjectileBehaviorOnCollisionPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ArrowProjectileBehaviorOnCollisionPatcher"/> class.</summary>
    internal ArrowProjectileBehaviorOnCollisionPatcher()
    {
        this.Target = "Archery.Framework.Objects.Projectiles.ArrowProjectile"
            .ToType()
            .RequireMethod("behaviorOnCollision");
    }

    #region harmony patches

    /// <summary>Adds overcharged piercing effect to arrows.</summary>
    [HarmonyPostfix]
    private static void ArrowProjectileBehaviorOnCollisionWithMonsterPostfix(
        BasicProjectile __instance, ref bool __result, ref int ____collectiveDamage, ref float ____knockback)
    {
        if (!__instance.Get_DidPierce())
        {
            return;
        }

        ____collectiveDamage = (int)(____collectiveDamage * 0.65f);
        ____knockback *= 0.65f;
        __instance.ModiftyOvercharge(0.65f);
        Reflector.GetUnboundFieldGetter<Projectile, NetFloat>(__instance, "xVelocity")
            .Invoke(__instance).Value *= 0.65f;
        Reflector.GetUnboundFieldGetter<Projectile, NetFloat>(__instance, "yVelocity")
            .Invoke(__instance).Value *= 0.65f;
        __result = false;
        __instance.Set_DidPierce(false);
    }

    #endregion harmony patches
}
