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
[ModRequirement("PeacefulEnd.Archery", "Archery", "2.1.0")]
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

    /// <summary>Reduce projectile stats post-piercing.</summary>
    [HarmonyPostfix]
    private static void ArrowProjectileBehaviorOnCollisionWithMonsterPostfix(
        BasicProjectile __instance, ref bool __result, ref int ____collectiveDamage, ref float ____knockback)
    {
        if (!__instance.Get_DidPierce())
        {
            return;
        }

        __result = false;
        __instance.Set_DidPierce(false);
    }

    #endregion harmony patches
}
