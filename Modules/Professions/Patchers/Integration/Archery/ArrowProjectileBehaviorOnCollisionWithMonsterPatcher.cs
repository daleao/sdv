namespace DaLion.Overhaul.Modules.Professions.Patchers.Integration.Archery;

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
        this.Prefix!.before = new[] { OverhaulModule.Combat.Namespace };
    }

    #region harmony patches

    /// <summary>Desperado Ultimate charge + check for arrow pierce.</summary>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyBefore("DaLion.Overhaul.Modules.Combat")]
    private static bool ArrowProjectileBehaviorOnCollisionWithMonsterPrefix(
        BasicProjectile __instance, ref (int, float, float, IUltimate?)? __state, ref int ____collectiveDamage, Farmer ____owner, NPC n)
    {
        if (n is not Monster { IsMonster: true } monster || monster.isInvincible())
        {
            return false; // don't run original logic
        }

        if (!____owner.HasProfession(Profession.Desperado))
        {
            return true; // run original logic
        }

        var overcharge = __instance.Get_Overcharge();
        var ogDamage = ____collectiveDamage;
        var monsterResistanceModifier = 1f + (monster.resilience.Value / 10f);
        var inverseResistanceModifer = 1f / monsterResistanceModifier;
        IUltimate? ultimate = ____owner.IsLocalPlayer && ProfessionsModule.Config.EnableLimitBreaks
            ? ____owner.Get_Ultimate()
            : null;

        // check for quick shot
        if (ProfessionsModule.State.LastDesperadoTarget is not null &&
            monster != ProfessionsModule.State.LastDesperadoTarget)
        {
            Log.D("Did quick shot!");
            ____collectiveDamage = (int)(____collectiveDamage * 1.5f);
            if (ultimate is DeathBlossom { IsActive: false })
            {
                ultimate.ChargeValue += 12d;
            }
        }
        // check for pierce, which is mutually exclusive with quick shot
        else if (____owner.HasProfession(Profession.Desperado, true) && __instance.Get_CanPierce() &&
                 Game1.random.NextDouble() < (overcharge - 1.5f) * inverseResistanceModifer)
        {
            Log.D("Pierced!");
            __instance.Set_DidPierce(true);
            if (CombatModule.Config.NewResistanceFormula)
            {
                ____collectiveDamage = (int)(____collectiveDamage * monsterResistanceModifier);
            }
            else
            {
                ____collectiveDamage += monster.resilience.Value;
            }
        }

        __state = (ogDamage, overcharge, inverseResistanceModifer, ultimate);
        Log.D("Pierced!");
        return true; // run original logic
    }

    /// <summary>Reduce projectile stats post-pierce.</summary>
    [HarmonyPostfix]
    private static void ArrowProjectileBehaviorOnCollisionWithMonsterPostfix(
        BasicProjectile __instance, (int, float, float, IUltimate?)? __state, ref int ____collectiveDamage, ref float ____knockback, Farmer ____owner, NPC n)
    {
        if (!__state.HasValue)
        {
            return;
        }

        ____collectiveDamage = __state.Value.Item1;
        if (__state.Value.Item2 > 1f)
        {
            ____collectiveDamage = (int)(____collectiveDamage * __state.Value.Item3);
            ____knockback *= __state.Value.Item3;
            __instance.ModiftyOvercharge(__state.Value.Item3);
            Reflector.GetUnboundFieldGetter<Projectile, NetFloat>("xVelocity")
                .Invoke(__instance).Value *= __state.Value.Item3;
            Reflector.GetUnboundFieldGetter<Projectile, NetFloat>("yVelocity")
                .Invoke(__instance).Value *= __state.Value.Item3;
        }

        // Desperado checks
        if (____owner.HasProfession(Profession.Desperado) && n is Monster { IsMonster: true } monster)
        {
            ProfessionsModule.State.LastDesperadoTarget = monster;
        }

        // increment ultimate meter
        if (__state.Value.Item4 is DeathBlossom { IsActive: false } ultimate && __state.Value.Item2 >= 1f)
        {
            ultimate.ChargeValue += __state.Value.Item2 * 8d;
        }
    }

    #endregion injected subroutines
}
