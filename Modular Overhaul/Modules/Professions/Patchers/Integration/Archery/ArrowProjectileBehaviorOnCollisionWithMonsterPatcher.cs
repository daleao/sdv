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

    /// <summary>Adds Rascal ammo recovery + Desperado Ultimate charge + check for piercing effect.</summary>
    [HarmonyPrefix]
    private static void ArrowProjectileBehaviorOnCollisionWithMonsterPrefix(
        BasicProjectile __instance, ref float ____breakChance, Farmer ____owner)
    {
        if (!____owner.HasProfession(Profession.Rascal))
        {
            return;
        }

        ____breakChance /= ____owner.HasProfession(Profession.Rascal, true) ? 1.7f : 1.35f;
        if (!____owner.HasProfession(Profession.Desperado))
        {
            return;
        }

        var overcharge = __instance.Get_Overcharge();
        ____breakChance *= 2f - overcharge;

        if (____owner.IsLocalPlayer && ____owner.Get_Ultimate() is DeathBlossom { IsActive: false } blossom &&
            ProfessionsModule.Config.EnableLimitBreaks)
        {
            blossom.ChargeValue += (__instance.Get_DidPierce() ? 18 : 12) - (10 * ____owner.health / ____owner.maxHealth);
        }

        if (!__instance.Get_CanPierce())
        {
            return;
        }

        var pierceChance = __instance.Get_Overcharge() - 1f;
        if (Game1.random.NextDouble() > pierceChance)
        {
            return;
        }

        __instance.Set_DidPierce(true);
    }

    /// <summary>Add overcharged shot effects.</summary>
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction>? ArrowProjectileBehaviorOnCollisionWithMonsterTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase original)
    {
        if (SlingshotsModule.ShouldEnable)
        {
            return null;
        }

        var helper = new ILHelper(original, instructions);
        try
        {
            helper
                .Match(
                    new[]
                    {
                        new CodeInstruction(
                            OpCodes.Callvirt,
                            typeof(GameLocation).RequireMethod(
                                nameof(GameLocation.damageMonster),
                                new[]
                                {
                                    typeof(Rectangle), typeof(int), typeof(int), typeof(bool), typeof(float),
                                    typeof(int), typeof(float), typeof(float), typeof(bool), typeof(Farmer),
                                })),
                    })
                .Insert(
                    new[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0), new CodeInstruction(OpCodes.Ldarg_2),
                        new CodeInstruction(
                            OpCodes.Call,
                            typeof(ArrowProjectileBehaviorOnCollisionWithMonsterPatcher).RequireMethod(
                                nameof(DamageMonsterWrapper))),
                    });
        }
        catch (Exception ex)
        {
            Log.E($"Failed injecting bow damage monster wrapper.\nHelper returned {ex}");
            return null;
        }

        return helper.Flush();
    }

    #endregion harmony patches

    #region injected subroutines

    private static void DamageMonsterWrapper(
        Rectangle monsterBox,
        int minDamage,
        int maxDamage,
        bool isBomb,
        float knocback,
        int precision,
        float critChance,
        float critPower,
        bool triggerMonsterInvincibilityTimer,
        Farmer firer,
        BasicProjectile projectile,
        GameLocation location)
    {
        if (!firer.HasProfession(Profession.Desperado))
        {
            return;
        }

        var source = projectile.Get_Source();
        if (source is null)
        {
            return;
        }

        var weaponData = ArcheryIntegration.Instance!.ModApi!.GetWeaponData(Manifest, source);
        if (!weaponData.Key)
        {
            return;
        }

        if (weaponData.Value.WeaponType == WeaponType.Crossbow)
        {
            location.damageMonster(
                monsterBox,
                minDamage,
                maxDamage,
                false,
                knocback,
                0,
                critChance, 
                critPower,
                true,
                firer);
        }
        else
        {
            var overcharge = projectile.Get_Overcharge();
            location.damageMonster(
                monsterBox,
                (int)(minDamage * overcharge),
                (int)(maxDamage * overcharge),
                false,
                knocback * overcharge,
                0,
                critChance,
                critPower,
                true,
                firer);
        }
    }

    #endregion injected subroutines
}
