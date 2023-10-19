namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.Archery;

#region using directives

using DaLion.Overhaul.Modules.Professions.VirtualProperties;
using DaLion.Shared.Attributes;
using DaLion.Shared.Extensions.Reflection;
using DaLion.Shared.Harmony;
using HarmonyLib;
using Netcode;
using StardewValley.Monsters;
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

    /// <summary>Reset pierce flag.</summary>
    [HarmonyPostfix]
    private static void ArrowProjectileBehaviorOnCollision(BasicProjectile __instance, ref bool __result)
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
