namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration;

#region using directives

using DaLion.Overhaul.Modules.Professions.Extensions;
using DaLion.Overhaul.Modules.Professions.Ultimates;
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
internal sealed class ArrowProjectileBehaviorOnCollisionWithMonsterPatcher : HarmonyPatcher
{
    /// <summary>Initializes a new instance of the <see cref="ArrowProjectileBehaviorOnCollisionWithMonsterPatcher"/> class.</summary>
    internal ArrowProjectileBehaviorOnCollisionWithMonsterPatcher()
    {
        this.Target = "Archery.Framework.Objects.Projectiles.ArrowProjectile"
            .ToType()
            .RequireMethod("behaviorOnCollisionWithMonster");
        this.Prefix!.priority = Priority.High;
        this.Prefix!.before = new[] { OverhaulModule.Slingshots.Namespace };
    }

    #region harmony patches

    /// <summary>Desperado Ultimate charge + check for piercing effect.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("DaLion.Overhaul.Modules.Slingshots")]
    private static bool ArrowProjectileBehaviorOnCollisionWithMonsterPrefix(
        BasicProjectile __instance, ref int ____collectiveDamage, ref float ____knockback, Farmer ____owner, NPC n)
    {
        if (n is not Monster monster || monster.isInvincible())
        {
            return false; // don't run original logic
        }

        if (!____owner.HasProfession(Profession.Desperado))
        {
            return true; // run original logic
        }

        if (____owner.IsLocalPlayer && ____owner.Get_Ultimate() is DeathBlossom { IsActive: false } blossom &&
            ProfessionsModule.Config.EnableLimitBreaks)
        {
            blossom.ChargeValue +=
                (__instance.Get_DidPierce() ? 18 : 12) - (10 * ____owner.health / ____owner.maxHealth);
        }

        if (!__instance.Get_CanPierce())
        {
            return true; // run original logic
        }

        var pierceChance = __instance.Get_Overcharge() - 1f;
        if (Game1.random.NextDouble() > pierceChance)
        {
            return true; // run original logic
        }

        ____collectiveDamage = (int)(____collectiveDamage * 0.65f);
        ____knockback *= 0.65f;
        __instance.ModiftyOvercharge(0.65f);
        Reflector.GetUnboundFieldGetter<Projectile, NetFloat>(__instance, "xVelocity")
            .Invoke(__instance).Value *= 0.65f;
        Reflector.GetUnboundFieldGetter<Projectile, NetFloat>(__instance, "yVelocity")
            .Invoke(__instance).Value *= 0.65f;
        __instance.Set_DidPierce(true);
        Log.D("Pierced!");
        return true; // run original logic
    }

    #endregion injected subroutines
}
